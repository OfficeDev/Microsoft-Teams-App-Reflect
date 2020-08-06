// -----------------------------------------------------------------------
// <copyright file="MessageExtension.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Teams.Apps.Reflect.Web
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using AdaptiveCards;
    using Bogus;
    using Microsoft.ApplicationInsights;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Teams;
    using Microsoft.Bot.Connector;
    using Microsoft.Bot.Schema;
    using Microsoft.Bot.Schema.Teams;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Reflection.Interfaces;
    using Reflection.Model;
    using Reflection.Repositories.FeedbackData;
    using Reflection.Repositories.QuestionsData;
    using Reflection.Repositories.RecurssionData;
    using Reflection.Repositories.ReflectionData;

    /// <summary>
    /// MessageExtension.
    /// </summary>
    public class MessageExtension : TeamsActivityHandler
    {
        private readonly IConfiguration _configuration;
        private readonly TelemetryClient _telemetry;
        private readonly ICard _cardHelper;
        private readonly IDataBase _dbHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageExtension"/> class.
        /// Message Extension.
        /// </summary>
        public MessageExtension(IConfiguration configuration, TelemetryClient telemetry, ICard cardHelper, IDataBase dbHelper)
        {
            _configuration = configuration;
            _telemetry = telemetry;
            _cardHelper = cardHelper;
            _dbHelper = dbHelper;
        }

        /// <summary>
        /// On message activity sync.
        /// </summary>
        /// <param name="turnContext">turnContext.</param>
        /// <param name="cancellationToken">cancellationToken.</param>
        /// <returns>.</returns>
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            _telemetry.TrackEvent("OnMessageActivityAsync");

            try
            {
                FeedbackDataRepository feedbackDataRepository = new FeedbackDataRepository(_configuration, _telemetry);
                ReflectionDataRepository reflectionDataRepository = new ReflectionDataRepository(_configuration, _telemetry);
                RecurssionDataRepository recurssionDataRepository = new RecurssionDataRepository(_configuration, _telemetry);
                QuestionsDataRepository questiondatarepository = new QuestionsDataRepository(_configuration, _telemetry);

                if (turnContext.Activity.Value != null)
                {
                    var response = JsonConvert.DeserializeObject<UserfeedbackInfo>(turnContext.Activity.Value.ToString());
                    var reply = Activity.CreateMessageActivity();
                    if (response.type == ReflectConstants.SaveFeedBack)
                    {
                        var name = (turnContext.Activity.From.Name).Split();
                        response.userName = name[0] + ' ' + name[1];
                        response.emailId = await _dbHelper.GetUserEmailId(turnContext);

                        // Check if this is user's second feedback
                        FeedbackDataEntity feebackData = await feedbackDataRepository.GetReflectionFeedback(response.reflectionId, response.emailId);
                        if (feebackData != null && response.emailId == feebackData.FeedbackGivenBy)
                        {
                            feebackData.Feedback = response.feedbackId;
                            await feedbackDataRepository.CreateOrUpdateAsync(feebackData);
                        }
                        else
                        {
                            await _dbHelper.SaveReflectionFeedbackDataAsync(response);
                        }

                        try
                        {
                            // Check if message id is present in reflect data
                            ReflectionDataEntity reflectData = await reflectionDataRepository.GetReflectionData(response.reflectionId);
                            QuestionsDataEntity question = await questiondatarepository.GetQuestionData(reflectData.QuestionID);
                            Dictionary<int, List<FeedbackDataEntity>> feedbacks = await feedbackDataRepository.GetReflectionFeedback(response.reflectionId);
                            var adaptiveCard = _cardHelper.FeedBackCard(feedbacks, response.reflectionId, question.Question);
                            TaskInfo taskInfo = new TaskInfo();
                            taskInfo.question = question.Question;
                            taskInfo.postCreateBy = reflectData.CreatedBy;
                            taskInfo.privacy = reflectData.Privacy;
                            taskInfo.reflectionID = reflectData.ReflectionID;
                            Attachment attachment = new Attachment()
                            {
                                ContentType = AdaptiveCard.ContentType,
                                Content = adaptiveCard
                            };
                            reply.Attachments.Add(attachment);
                            if (reflectData.MessageID == null)
                            {
                                var result = turnContext.SendActivityAsync(reply, cancellationToken);
                                reflectData.MessageID = result.Result.Id;
                                // update message-id in reflection table
                                await reflectionDataRepository.InsertOrMergeAsync(reflectData);

                            }
                            else
                            {
                                reply.Id = reflectData.MessageID;
                                await turnContext.UpdateActivityAsync(reply);


                            }
                        }
                        catch (System.Exception e)
                        {
                            _telemetry.TrackException(e);
                            Console.WriteLine(e.Message.ToString());
                        }
                    }
                }
                else
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Hello from the TeamsMessagingExtensionsActionPreviewBot."), cancellationToken);
                }

            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex);
            }
        }

        /// <summary>
        /// On Teams Task Module Submit Async.
        /// </summary>
        /// <param name="turnContext">turnContext.</param>
        /// <param name="taskModuleRequest">taskModuleRequest.</param>
        /// <param name="cancellationToken">cancellationToken.</param>
        /// <returns>.</returns>
        protected override async Task<TaskModuleResponse> OnTeamsTaskModuleSubmitAsync(ITurnContext<IInvokeActivity> turnContext, TaskModuleRequest taskModuleRequest, CancellationToken cancellationToken)
        {
            ReflectionDataRepository reflectionDataRepository = new ReflectionDataRepository(_configuration, _telemetry);
            QuestionsDataRepository questiondatarepository = new QuestionsDataRepository(_configuration, _telemetry);
            FeedbackDataRepository feedbackDataRepository = new FeedbackDataRepository(_configuration, _telemetry);
            try
            {
                TaskInfo taskInfo = JsonConvert.DeserializeObject<TaskInfo>(taskModuleRequest.Data.ToString());
                var reply = Activity.CreateMessageActivity();

                // Check if message id is present in reflect data
                ReflectionDataEntity reflectData = await reflectionDataRepository.GetReflectionData(taskInfo.reflectionID);
                QuestionsDataEntity question = await questiondatarepository.GetQuestionData(reflectData.QuestionID);
                Dictionary<int, List<FeedbackDataEntity>> feedbacks = await feedbackDataRepository.GetReflectionFeedback(taskInfo.reflectionID);
                var adaptiveCard = _cardHelper.FeedBackCard(feedbacks, taskInfo.reflectionID, question.Question);

                Attachment attachment = new Attachment()
                {
                    ContentType = AdaptiveCard.ContentType,
                    Content = adaptiveCard
                };
                reply.Attachments.Add(attachment);
                if (reflectData.MessageID == null)
                {

                    var result = turnContext.SendActivityAsync(reply, cancellationToken);
                    reflectData.MessageID = result.Result.Id;

                    // update messageid in reflection table
                    await reflectionDataRepository.InsertOrMergeAsync(reflectData);

                }
                else
                {
                    reply.Id = reflectData.MessageID;
                    await turnContext.UpdateActivityAsync(reply);


                }
                return null;
            }
            catch (System.Exception e)
            {
                _telemetry.TrackException(e);
                Console.WriteLine(e.Message.ToString());
                return null;
            }
        }

        /// <summary>
        /// On Invoke Activity Async.
        /// </summary>
        /// <param name="turnContext">turnContext.</param>
        /// <param name="cancellationToken">cancellationToken.</param>
        /// <returns>.</returns>
        protected override async Task<InvokeResponse> OnInvokeActivityAsync(ITurnContext<IInvokeActivity> turnContext, CancellationToken cancellationToken)
        {
            if (turnContext.Activity.Name == "custom")
            {
                return new InvokeResponse() { Status = 200 };
            }

            return await base.OnInvokeActivityAsync(turnContext, cancellationToken);
        }

        /// <summary>
        /// On Teams Task Module Fetch Async.
        /// </summary>
        /// <param name="turnContext">turnContext.</param>
        /// <param name="taskModuleRequest">taskModuleRequest.</param>
        /// <param name="cancellationToken">cancellationToken.</param>
        /// <returns>.</returns>
        protected override async Task<TaskModuleResponse> OnTeamsTaskModuleFetchAsync(ITurnContext<IInvokeActivity> turnContext, TaskModuleRequest taskModuleRequest, CancellationToken cancellationToken)
        {
            _telemetry.TrackEvent("OnTeamsTaskModuleFetchAsync");
            try
            {
                ReflctionData reldata = JsonConvert.DeserializeObject<ReflctionData>(taskModuleRequest.Data.ToString());
                FeedbackDataRepository feedbackDataRepository = new FeedbackDataRepository(_configuration, _telemetry);
                ReflectionDataRepository reflectionDataRepository = new ReflectionDataRepository(_configuration, _telemetry);
                RecurssionDataRepository recurssionDataRepository = new RecurssionDataRepository(_configuration, _telemetry);
                QuestionsDataRepository questiondatarepository = new QuestionsDataRepository(_configuration, _telemetry);
                var response = new UserfeedbackInfo();
                var name = (turnContext.Activity.From.Name).Split();
                response.userName = name[0] + ' ' + name[1];
                response.emailId = await _dbHelper.GetUserEmailId(turnContext);
                var reflectionid = reldata.datajson.ReflectionId;
                var feedbackId = reldata.datajson.FeedbackId;
                response.reflectionId = reflectionid;
                response.feedbackId = feedbackId;

                // Check if this is user's second feedback
                FeedbackDataEntity feebackData = await feedbackDataRepository.GetReflectionFeedback(response.reflectionId, response.emailId);
                TaskInfo taskInfo = new TaskInfo();
                if (response.feedbackId != 0)
                {
                    if (feebackData != null && response.emailId == feebackData.FeedbackGivenBy)
                    {
                        feebackData.Feedback = response.feedbackId;
                        await feedbackDataRepository.CreateOrUpdateAsync(feebackData);
                    }
                    else
                    {
                        await _dbHelper.SaveReflectionFeedbackDataAsync(response);
                    }

                    try
                    {
                        // Check if message id is present in reflect data
                        ReflectionDataEntity reflectData = await reflectionDataRepository.GetReflectionData(response.reflectionId);
                        QuestionsDataEntity question = await questiondatarepository.GetQuestionData(reflectData.QuestionID);
                        Dictionary<int, List<FeedbackDataEntity>> feedbacks = await feedbackDataRepository.GetReflectionFeedback(response.reflectionId);
                        var adaptiveCard = _cardHelper.FeedBackCard(feedbacks, response.reflectionId, question.Question);

                        taskInfo.question = question.Question;
                        taskInfo.postCreateBy = reflectData.CreatedBy;
                        taskInfo.privacy = reflectData.Privacy;
                        taskInfo.reflectionID = reflectData.ReflectionID;
                        Attachment attachment = new Attachment()
                        {
                            ContentType = AdaptiveCard.ContentType,
                            Content = adaptiveCard,
                        };
                        var reply = Activity.CreateMessageActivity();
                        reply.Attachments.Add(attachment);
                        if (reflectData.MessageID == null)
                        {
                            var result = turnContext.SendActivityAsync(reply, cancellationToken);
                            reflectData.MessageID = result.Result.Id;

                            // update messageid in reflection table
                            await reflectionDataRepository.InsertOrMergeAsync(reflectData);
                        }
                        else
                        {
                            reply.Id = reflectData.MessageID;
                            await turnContext.UpdateActivityAsync(reply);
                        }
                    }
                    catch (System.Exception e)
                    {
                        _telemetry.TrackException(e);
                        Console.WriteLine(e.Message.ToString());
                    }
                }

                return new TaskModuleResponse
                {
                    Task = new TaskModuleContinueResponse
                    {
                        Value = new TaskModuleTaskInfo()
                        {
                            Height = 600,
                            Width = 600,
                            Title = "Make space for people to share how they feel",
                            Url = reldata.data.URL + reflectionid + '/' + feedbackId + '/' + response.userName,
                        },
                    },
                };
            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex);
                return null;
            }
        }

        /// <summary>
        /// On Teams Messaging Extension Submit Action Async.
        /// </summary>
        /// <param name="turnContext">turnContext.</param>
        /// <param name="action">action.</param>
        /// <param name="cancellationToken">cancellationToken.</param>
        /// <returns>.</returns>
        protected override async Task<MessagingExtensionActionResponse> OnTeamsMessagingExtensionSubmitActionAsync(ITurnContext<IInvokeActivity> turnContext, MessagingExtensionAction action, CancellationToken cancellationToken)
        {
            _telemetry.TrackEvent("OnTeamsMessagingExtensionSubmitActionAsync");
            ReflectionDataRepository reflectionDataRepository = new ReflectionDataRepository(_configuration, _telemetry);
            try
            {
                TaskInfo taskInfo = JsonConvert.DeserializeObject<TaskInfo>(action.Data.ToString());
                switch (taskInfo.action)
                {
                    case "reflection":
                        return await OnTeamsMessagingExtensionFetchTaskAsync(turnContext, action, cancellationToken);
                    case "sendAdaptiveCard":
                        try
                        {
                            var name = (turnContext.Activity.From.Name).Split();
                            taskInfo.postCreateBy = name[0] + ' ' + name[1];
                            taskInfo.postCreatedByEmail = await _dbHelper.GetUserEmailId(turnContext);
                            taskInfo.channelID = turnContext.Activity.TeamsGetChannelId();
                            taskInfo.postSendNowFlag = (taskInfo.executionTime == "Send now") ? true : false;
                            taskInfo.IsActive = true;
                            taskInfo.questionRowKey = Guid.NewGuid().ToString();
                            taskInfo.recurrsionRowKey = Guid.NewGuid().ToString();
                            taskInfo.reflectionRowKey = Guid.NewGuid().ToString();
                            taskInfo.serviceUrl = turnContext.Activity.ServiceUrl;
                            taskInfo.teantId = turnContext.Activity.Conversation.TenantId;
                            taskInfo.scheduleId = Guid.NewGuid().ToString();
                            await _dbHelper.SaveReflectionDataAsync(taskInfo);
                            if (taskInfo.postSendNowFlag == true)
                            {
                                var typingActivity = MessageFactory.Text(string.Empty);
                                typingActivity.Type = ActivityTypes.Typing;
                                await turnContext.SendActivityAsync(typingActivity);
                                var adaptiveCard = _cardHelper.CreateNewReflect(taskInfo);
                                var message = MessageFactory.Attachment(new Attachment { ContentType = AdaptiveCard.ContentType, Content = adaptiveCard });
                                var resultid = await turnContext.SendActivityAsync(message, cancellationToken);
                                ReflectionDataEntity reflectData = await reflectionDataRepository.GetReflectionData(taskInfo.reflectionID);
                                reflectData.ReflectMessageId = resultid.Id;
                                await reflectionDataRepository.InsertOrMergeAsync(reflectData);
                                try
                                {

                                    var feedbackCard = _cardHelper.FeedBackCard(new Dictionary<int, List<FeedbackDataEntity>>(), taskInfo.reflectionID, taskInfo.question);

                                    Attachment attachmentfeedback = new Attachment()
                                    {
                                        ContentType = AdaptiveCard.ContentType,
                                        Content = feedbackCard,
                                    };
                                    using var connector = new ConnectorClient(new Uri(reflectData.ServiceUrl), _configuration["MicrosoftAppId"], _configuration["MicrosoftAppPassword"]);

                                    var conversationId = $"{reflectData.ChannelID};messageid={reflectData.ReflectMessageId}";
                                    var replyActivity = MessageFactory.Attachment(new Attachment { ContentType = AdaptiveCard.ContentType, Content = feedbackCard });
                                    replyActivity.Conversation = new ConversationAccount(id: conversationId);
                                    var resultfeedback = await connector.Conversations.SendToConversationAsync((Activity)replyActivity, cancellationToken);
                                    reflectData.MessageID = resultfeedback.Id;
                                    // update messageid in reflection table
                                    await reflectionDataRepository.InsertOrMergeAsync(reflectData);
                                }
                                catch (System.Exception e)
                                {
                                    _telemetry.TrackException(e);
                                    Console.WriteLine(e.Message.ToString());
                                }
                            }
                            return null;
                        }
                        catch (Exception ex)
                        {
                            _telemetry.TrackException(ex);
                            return null;
                        }

                    case "ManageRecurringPosts":
                        var postCreatedByEmail = await _dbHelper.GetUserEmailId(turnContext);
                        var response = new MessagingExtensionActionResponse()
                        {
                            Task = new TaskModuleContinueResponse()
                            {
                                Value = new TaskModuleTaskInfo()
                                {
                                    Height = 600,
                                    Width = 780,
                                    Title = "Make space for people to share how they feel",
                                    Url = this._configuration["BaseUri"] + "/ManageRecurringPosts/" + postCreatedByEmail + "?pathfromindex=true"
                                },
                            },
                        };
                        return response;

                    case "OpenDetailfeedback":
                        var responsefeedback = new MessagingExtensionActionResponse()
                        {
                            Task = new TaskModuleContinueResponse()
                            {
                                Value = new TaskModuleTaskInfo()
                                {
                                    Height = 600,
                                    Width = 780,
                                    Title = "Make space for people to share how they feel",
                                    Url = this._configuration["BaseUri"] + "/openReflectionFeedback/" + taskInfo.reflectionID + "/" + taskInfo.feedback,
                                },
                            },
                        };
                        return responsefeedback;
                    case "removeposts":
                        try
                        {
                            var activity = Activity.CreateMessageActivity();
                            if (taskInfo.isDelete)
                            {
                                var messageId = await _dbHelper.RemoveReflectionId(taskInfo.messageID);
                                if (messageId != null)
                                {
                                    await turnContext.DeleteActivityAsync(messageId);

                                }
                                await turnContext.DeleteActivityAsync(taskInfo.messageID);
                                activity.Text = "This Reflect poll has been removed";
                                await turnContext.SendActivityAsync(activity);
                                return null;
                            }
                            else
                            {
                                return null;
                            }
                        }
                        catch (Exception ex)
                        {
                            _telemetry.TrackException(ex);
                            return null;
                        }

                    default:
                        return null;
                };
            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex);
                return null;
            }
        }

        /// <summary>
        /// On Teams Messaging Extension Fetch Task Async.
        /// </summary>
        /// <param name="turnContext">turnContext.</param>
        /// <param name="action">action.</param>
        /// <param name="cancellationToken">cancellationToken.</param>
        /// <returns>response.</returns>
        protected override async Task<MessagingExtensionActionResponse> OnTeamsMessagingExtensionFetchTaskAsync(ITurnContext<IInvokeActivity> turnContext, MessagingExtensionAction action, CancellationToken cancellationToken)
        {
            _telemetry.TrackEvent("OnTeamsMessagingExtensionFetchTaskAsync");

            try
            {
                var url = this._configuration["BaseUri"];
                if (action.CommandId == ReflectConstants.RecurringPosts)
                {
                    var postCreatedByEmail = await _dbHelper.GetUserEmailId(turnContext);
                    url = this._configuration["BaseUri"] + "/ManageRecurringPosts/" + postCreatedByEmail;
                    var response = new MessagingExtensionActionResponse()
                    {
                        Task = new TaskModuleContinueResponse()
                        {
                            Value = new TaskModuleTaskInfo()
                            {
                                Height = 620,
                                Width = 800,
                                Title = "Make space for people to share how they feel",
                                Url = url
                            },
                        },
                    };
                    return response;
                }
                else if (action.CommandId == ReflectConstants.RemovePosts)
                {
                    if (turnContext.Activity.Conversation.Id != null)
                    {
                        var replymessageid = turnContext.Activity.Conversation.Id.Split("=");
                        string messageId = replymessageid[1];
                        var confirmcard = _cardHelper.ConfirmationCard(messageId);

                        Attachment attachmentfeedback = new Attachment()
                        {
                            ContentType = AdaptiveCard.ContentType,
                            Content = confirmcard,
                        };
                        var response = new MessagingExtensionActionResponse()
                        {
                            Task = new TaskModuleContinueResponse()
                            {
                                Value = new TaskModuleTaskInfo()
                                {
                                    Height = 100,
                                    Width = 300,
                                    Title = "Make space for people to share how they feel",
                                    Card = attachmentfeedback
                                },
                            },
                        };
                        return response;

                    }

                    return null;
                }
                else if (action.CommandId == ReflectConstants.CreateReflect)
                {
                    var name = (turnContext.Activity.From.Name).Split();
                    var userName = name[0] + ' ' + name[1];
                    url = this._configuration["BaseUri"] + "/" + userName;
                    var response = new MessagingExtensionActionResponse()
                    {
                        Task = new TaskModuleContinueResponse()
                        {
                            Value = new TaskModuleTaskInfo()
                            {
                                Height = 620,
                                Width = 800,
                                Title = "Make space for people to share how they feel",
                                Url = url
                            },
                        },
                    };
                    return response;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex);
                return null;
            }
        }

        /// <summary>
        /// On Teams Messaging Extension Fetch Task Async.
        /// </summary>
        /// <param name="turnContext">turnContext.</param>
        /// <param name="query">action.</param>
        /// <param name="cancellationToken">cancellationToken.</param>
        /// <returns>response.</returns>
        protected override Task<MessagingExtensionResponse> OnTeamsMessagingExtensionQueryAsync(ITurnContext<IInvokeActivity> turnContext, MessagingExtensionQuery query, CancellationToken cancellationToken)
        {
            _telemetry.TrackEvent("OnTeamsMessagingExtensionQueryAsync");

            try
            {
                var title = "";
                var titleParam = query.Parameters?.FirstOrDefault(p => p.Name == "cardTitle");
                if (titleParam != null)
                {
                    title = titleParam.Value.ToString();
                }

                if (query == null || query.CommandId != "getRandomText")
                {
                    // We only process the 'getRandomText' queries with this message extension
                    throw new NotImplementedException($"Invalid CommandId: {query.CommandId}");
                }

                var attachments = new MessagingExtensionAttachment[5];

                for (int i = 0; i < 5; i++)
                {
                    attachments[i] = GetAttachment(title);
                }

                var result = new MessagingExtensionResponse
                {
                    ComposeExtension = new MessagingExtensionResult
                    {
                        AttachmentLayout = "list",
                        Type = "result",
                        Attachments = attachments.ToList()
                    },
                };
                return Task.FromResult(result);

            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex);
                return null;
            }
        }

        /// <summary>
        /// On Teams Messaging Extension Select Item Async.
        /// </summary>
        /// <param name="turnContext">turnContext.</param>
        /// <param name="query">action.</param>
        /// <param name="cancellationToken">cancellationToken.</param>
        /// <returns>Attachment.</returns>
        protected override Task<MessagingExtensionResponse> OnTeamsMessagingExtensionSelectItemAsync(ITurnContext<IInvokeActivity> turnContext, JObject query, CancellationToken cancellationToken)
        {
            _telemetry.TrackEvent("OnTeamsMessagingExtensionSelectItemAsync");

            try
            {
                return Task.FromResult(new MessagingExtensionResponse
                {
                    ComposeExtension = new MessagingExtensionResult
                    {
                        AttachmentLayout = "list",
                        Type = "result",
                        Attachments = new MessagingExtensionAttachment[]
                        {
                        new ThumbnailCard()
                        .ToAttachment()
                            .ToMessagingExtensionAttachment()
                        }
                    },
                });
            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex);
                return null;
            }
        }

        /// <summary>
        /// On Teams Messaging Extension Card Button Clicked Async.
        /// </summary>
        /// <param name="turnContext">turnContext.</param>
        /// <param name="cardData">cardData.</param>
        /// <param name="cancellationToken">cancellationToken.</param>
        /// <returns>Attachment.</returns>
        protected async override Task OnTeamsMessagingExtensionCardButtonClickedAsync(ITurnContext<IInvokeActivity> turnContext, JObject cardData, CancellationToken cancellationToken)
        {
            _telemetry.TrackEvent("OnTeamsMessagingExtensionCardButtonClickedAsync");
            try
            {
                var reply = MessageFactory.Text("OnTeamsMessagingExtensionCardButtonClickedAsync Value: " + JsonConvert.SerializeObject(turnContext.Activity.Value));
                await turnContext.SendActivityAsync(reply, cancellationToken);
            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex);
            }
        }

        /// <summary>
        /// Gets Attachment.
        /// </summary>
        /// <param name="title">title.</param>
        /// <returns>Attachment.</returns>
        private MessagingExtensionAttachment GetAttachment(string title)
        {
            _telemetry.TrackEvent("GetAttachment");

            try
            {
                var card = new ThumbnailCard
                {
                    Title = !string.IsNullOrWhiteSpace(title) ? title : new Faker().Lorem.Sentence(),
                    Text = new Faker().Lorem.Paragraph(),
                    Images = new List<CardImage> { new CardImage("http://lorempixel.com/640/480?rand=" + DateTime.Now.Ticks.ToString()) }
                };

                return card
                    .ToAttachment()
                    .ToMessagingExtensionAttachment();
            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex);
                return null;
            }
        }
    }
}
