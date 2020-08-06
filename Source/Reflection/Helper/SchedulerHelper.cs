// -----------------------------------------------------------------------
// <copyright file="SchedulerHelper.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Reflection.Helper
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using AdaptiveCards;
    using Microsoft.ApplicationInsights;
    using Microsoft.Bot.Schema;
    using Microsoft.Bot.Schema.Teams;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using Reflection.Interfaces;
    using Reflection.Model;
    using Reflection.Repositories.FeedbackData;
    using Reflection.Repositories.QuestionsData;
    using Reflection.Repositories.RecurssionData;
    using Reflection.Repositories.ReflectionData;

    /// <summary>
    /// Scheduler Helper.
    /// </summary>
    public class SchedulerHelper : IHostedService
    {
        private Timer _timer;
        private readonly TelemetryClient _telemetry;
        private readonly IConfiguration _configuration;
        private readonly ICard _cardHelper;
        private readonly IDataBase _dbHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="SchedulerHelper"/> class.
        /// Scheduler Helper.
        /// </summary>
        public SchedulerHelper(TelemetryClient telemetry, IConfiguration configuration, ICard cardHelper, IDataBase dbHelper)
        {
            _telemetry = telemetry;
            _configuration = configuration;
            _cardHelper = cardHelper;
            _dbHelper = dbHelper;
        }

        /// <summary>
        /// StartAsync.
        /// </summary>
        /// <param name="cancellationToken">cancellationToken.</param>
        /// <returns>.</returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _telemetry.TrackEvent("StartAsync");

            _timer = new Timer(RunJob, null, TimeSpan.Zero,
            TimeSpan.FromMinutes(1));
            return Task.CompletedTask;
        }

        /// <summary>
        /// RunJob.
        /// </summary>
        /// <param name="state">state.</param>
        private async void RunJob(object state)
        {
            _telemetry.TrackEvent("RunJob");
            try
            {
                ChannelAccount channelAccount = new ChannelAccount(_configuration["MicrosoftAppId"]);
                Attachment attachment = new Attachment();
                TeamsChannelData channelData = new TeamsChannelData() { Notification = new NotificationInfo(true) };
                RecurssionDataRepository recurssionDataRepository = new RecurssionDataRepository(_configuration, _telemetry);
                ReflectionDataRepository reflectionDataRepository = new ReflectionDataRepository(_configuration, _telemetry);
                QuestionsDataRepository questiondatarepository = new QuestionsDataRepository(_configuration, _telemetry);

                var recurssionData = await recurssionDataRepository.GetAllRecurssionData();
                foreach (RecurssionDataEntity recurssionDataEntity in recurssionData)
                {
                    var reflectionData = await reflectionDataRepository.GetReflectionData(recurssionDataEntity.ReflectionID);
                    var question = await questiondatarepository.GetQuestionData(reflectionData.QuestionID);

                    TaskInfo taskInfo = new TaskInfo();
                    taskInfo.question = question.Question;
                    taskInfo.postCreateBy = reflectionData.CreatedBy;
                    taskInfo.privacy = reflectionData.Privacy;
                    taskInfo.reflectionID = reflectionData.ReflectionID;
                    var newPostCard = _cardHelper.CreateNewReflect(taskInfo);
                    Attachment newPostCardAttachment = new Attachment()
                    {
                        ContentType = AdaptiveCard.ContentType,
                        Content = newPostCard
                    };
                    var PostCardFeedback = _cardHelper.FeedBackCard(new Dictionary<int, List<FeedbackDataEntity>>(), taskInfo.reflectionID, taskInfo.question); ;
                    Attachment PostCardFeedbackAttachment = new Attachment()
                    {
                        ContentType = AdaptiveCard.ContentType,
                        Content = PostCardFeedback
                    };
                    var proactiveNotification = await new ProactiveMessageHelper(_configuration).SendChannelNotification(channelAccount, reflectionData.ServiceUrl, reflectionData.ChannelID, "", newPostCardAttachment);
                    if (proactiveNotification.IsSuccessful && proactiveNotification.MessageId != null)
                    {
                        reflectionData.ReflectMessageId = proactiveNotification.MessageId.Split("=")[1];
                        var feedbackproactivemessage = await new ProactiveMessageHelper(_configuration).SendChannelNotification(channelAccount, reflectionData.ServiceUrl, reflectionData.ChannelID, "", PostCardFeedbackAttachment, reflectionData.ReflectMessageId);
                        if (feedbackproactivemessage.IsSuccessful && proactiveNotification.MessageId != null)
                        {
                            reflectionData.MessageID = feedbackproactivemessage.MessageId;
                            await _dbHelper.UpdateReflectionMessageIdAsync(reflectionData);
                        }
                    }

                    var calculatedNextExecutionDateTime = _dbHelper.GetCalculatedNextExecutionDateTimeAsync(recurssionDataEntity);
                    recurssionDataEntity.NextExecutionDate = null;
                    await recurssionDataRepository.CreateOrUpdateAsync(recurssionDataEntity);
                    if (calculatedNextExecutionDateTime != null)
                    {
                        ReflectionDataEntity newreflectionDataEntity = new ReflectionDataEntity();
                        RecurssionDataEntity newRecurssionDataEntity = new RecurssionDataEntity();
                        var reflectionid = Guid.NewGuid();
                        var recurrsionid = Guid.NewGuid();
                        newreflectionDataEntity = reflectionData;
                        newreflectionDataEntity.RowKey = Guid.NewGuid().ToString();
                        newreflectionDataEntity.ReflectionID = reflectionid;
                        newreflectionDataEntity.RefCreatedDate = DateTime.Now;
                        newreflectionDataEntity.RecurrsionID = recurrsionid;
                        await reflectionDataRepository.CreateAsync(newreflectionDataEntity);
                        newRecurssionDataEntity = recurssionDataEntity;
                        newRecurssionDataEntity.RowKey = Guid.NewGuid().ToString();
                        newRecurssionDataEntity.ReflectionID = reflectionid;
                        newRecurssionDataEntity.CreatedDate = DateTime.Now;
                        newRecurssionDataEntity.RecurssionID = recurrsionid;
                        newRecurssionDataEntity.NextExecutionDate = calculatedNextExecutionDateTime;
                        await recurssionDataRepository.CreateAsync(newRecurssionDataEntity);
                    }

                }
            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex);
            }
        }

        /// <summary>
        /// StopAsync.
        /// </summary>
        /// <param name="cancellationToken">cancellationToken.</param>
        /// <returns>.</returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _telemetry.TrackEvent("StopAsync");

            return Task.CompletedTask;
        }
    }
}
