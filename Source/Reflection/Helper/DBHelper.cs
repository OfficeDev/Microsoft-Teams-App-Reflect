// -----------------------------------------------------------------------
// <copyright file="DBHelper.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Reflection.Helper
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.ApplicationInsights;
    using Microsoft.Azure.Documents;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Connector;
    using Microsoft.Bot.Schema;
    using Microsoft.Bot.Schema.Teams;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json.Linq;
    using Reflection.Interfaces;
    using Reflection.Model;
    using Reflection.Repositories;
    using Reflection.Repositories.FeedbackData;
    using Reflection.Repositories.QuestionsData;
    using Reflection.Repositories.RecurssionData;
    using Reflection.Repositories.ReflectionData;

    /// <summary>
    /// DB Helper.
    /// </summary>
    public class DBHelper : IDataBase
    {
        private IConfiguration _configuration;
        private TelemetryClient _telemetry;
        List<string> weekdays = new List<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DBHelper"/> class.
        /// DB Helper.
        /// </summary>
        public DBHelper(IConfiguration configuration, TelemetryClient telemetry)
        {
            _configuration = configuration;
            _telemetry = telemetry;
            weekdays.Add("Sunday");
            weekdays.Add("Monday");
            weekdays.Add("Tuesday");
            weekdays.Add("Wednesday");
            weekdays.Add("Thursday");
            weekdays.Add("Friday");
            weekdays.Add("Saturday");
        }

        /// <summary>
        /// Save Reflection data in Table Storage based on different conditions.
        /// </summary>
        /// <param name="taskInfo">This parameter is a ViewModel.</param>
        /// <returns>Null.</returns>
        public async Task SaveReflectionDataAsync(TaskInfo taskInfo)
        {
            _telemetry.TrackEvent("SaveReflectionDataAsync");
            try
            {
                ReflectionDataRepository reflectionDataRepository = new ReflectionDataRepository(_configuration, _telemetry);
                QuestionsDataRepository questionsDataRepository = new QuestionsDataRepository(_configuration, _telemetry);

                if (taskInfo != null)
                {
                    taskInfo.reflectionID = Guid.NewGuid();
                    taskInfo.recurssionID = Guid.NewGuid();
                    if (taskInfo.questionID == null)
                    {
                        taskInfo.questionID = Guid.NewGuid();
                    }

                    ReflectionDataEntity reflectEntity = new ReflectionDataEntity
                    {
                        ReflectionID = taskInfo.reflectionID,
                        PartitionKey = PartitionKeyNames.ReflectionDataTable.ReflectionDataPartition,
                        RowKey = taskInfo.reflectionRowKey,
                        CreatedBy = taskInfo.postCreateBy,
                        CreatedByEmail = taskInfo.postCreatedByEmail,
                        RefCreatedDate = DateTime.Now,
                        QuestionID = taskInfo.questionID,
                        Privacy = taskInfo.privacy,
                        RecurrsionID = taskInfo.recurssionID,
                        ChannelID = taskInfo.channelID,
                        MessageID = taskInfo.messageID,
                        SendNowFlag = taskInfo.postSendNowFlag,
                        IsActive = taskInfo.IsActive,
                        ReflectMessageId = taskInfo.reflectMessageId,
                        TenantId = taskInfo.teantId,
                        ServiceUrl = taskInfo.serviceUrl,
                        ScheduleId=taskInfo.scheduleId
                    };
                    await reflectionDataRepository.InsertOrMergeAsync(reflectEntity);

                    if (await questionsDataRepository.IsQuestionAlreadyPresent(taskInfo.question, taskInfo.postCreatedByEmail) == false)
                    {
                        await SaveQuestionsDataAsync(taskInfo);
                    }
                    else
                    {
                        var ques = await questionsDataRepository.GetQuestionData(taskInfo.questionID);
                        taskInfo.questionRowKey = ques.RowKey;
                    }

                    if (!(taskInfo.postSendNowFlag == true))
                    {
                        await SaveRecurssionDataAsync(taskInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex);
            }
        }

        /// <summary>
        /// Updates reflection message id.
        /// </summary>
        /// <param name="reflectionDataEntity">reflectionDataEntity.</param>
        /// <returns>Null.</returns>
        public async Task UpdateReflectionMessageIdAsync(ReflectionDataEntity reflectionDataEntity)
        {
            _telemetry.TrackEvent("SaveReflectionMessageIdAsync");
            try
            {
                ReflectionDataRepository reflectionDataRepository = new ReflectionDataRepository(_configuration, _telemetry);
                await reflectionDataRepository.CreateOrUpdateAsync(reflectionDataEntity);
            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex);
            }
        }

        /// <summary>
        /// Save Question data in Table Storage based on different conditions.
        /// </summary>
        /// <param name="taskInfo">This parameter is a ViewModel.</param>
        /// <returns>Null.</returns>
        public async Task SaveQuestionsDataAsync(TaskInfo taskInfo)
        {
            _telemetry.TrackEvent("DeleteReflections");
            try
            {
                QuestionsDataRepository questionsDataRepository = new QuestionsDataRepository(_configuration, _telemetry);
                QuestionsDataEntity questionEntity = new QuestionsDataEntity
                {
                    QuestionID = taskInfo.questionID,
                    PartitionKey = PartitionKeyNames.QuestionsDataTable.QuestionsDataPartition,
                    RowKey = taskInfo.questionRowKey,
                    Question = taskInfo.question,
                    IsDefaultFlag = false,
                    QuestionCreatedDate = DateTime.Now,
                    CreatedBy = taskInfo.postCreateBy,
                    CreatedByEmail = taskInfo.postCreatedByEmail,
                };

                await questionsDataRepository.CreateOrUpdateAsync(questionEntity);
            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex);
            }
        }

        /// <summary>
        /// Save Reflection Recursion data in Table Storage.
        /// </summary>
        /// <param name="taskInfo">This parameter is a ViewModel.</param>
        /// <returns>Null.</returns>
        public async Task SaveRecurssionDataAsync(TaskInfo taskInfo)
        {
            _telemetry.TrackEvent("SaveRecurssionDataAsync");
            try
            {
                RecurssionDataRepository recurssionDataRepository = new RecurssionDataRepository(_configuration, _telemetry);

                RecurssionDataEntity recurssionEntity = new RecurssionDataEntity
                {
                    RecurssionID = taskInfo.recurssionID,
                    PartitionKey = PartitionKeyNames.RecurssionDataTable.RecurssionDataPartition,
                    RowKey = taskInfo.recurrsionRowKey,
                    ReflectionRowKey = taskInfo.reflectionRowKey,
                    QuestionRowKey = taskInfo.questionRowKey,
                    ReflectionID = taskInfo.reflectionID,
                    RecursstionType = taskInfo.recurssionType,
                    CustomRecurssionTypeValue = taskInfo.customRecurssionTypeValue,
                    CreatedDate = DateTime.Now,
                    ExecutionDate = taskInfo.executionDate,
                    ExecutionTime = taskInfo.executionTime,
                    RecurssionEndDate = taskInfo.executionDate.AddDays(60),
                    NextExecutionDate = taskInfo.nextExecutionDate,
                    ScheduleId=taskInfo.scheduleId
                };
                await recurssionDataRepository.CreateOrUpdateAsync(recurssionEntity);
            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex);
            }
        }

        /// <summary>
        /// Get the next working day.
        /// </summary>
        /// <returns>next week day.</returns>
        public DateTime GetNextWeekday()
        {
            DateTime nextWorkingDay = DateTime.UtcNow.AddDays(1);
            while (nextWorkingDay.DayOfWeek == DayOfWeek.Saturday || nextWorkingDay.DayOfWeek == DayOfWeek.Sunday)
            {
                nextWorkingDay = nextWorkingDay.AddDays(1);
            }

            return nextWorkingDay;
        }

        /// <summary>
        /// Get the next working day.
        /// </summary>
        /// <param name="day">day.</param>
        /// <returns>next working day.</returns>
        public DateTime GetNextWeeklyday(DayOfWeek day)
        {
            DateTime nextWeeklyday = DateTime.UtcNow.AddDays(1);
            while (nextWeeklyday.DayOfWeek != day)
            {
                nextWeeklyday = nextWeeklyday.AddDays(1);
            }

            return nextWeeklyday;
        }

        /// <summary>
        /// Get the calculated next execution date time.
        /// </summary>
        /// <param name="recurssionEntity">recurssionEntity.</param>
        /// <returns>Calculated next execution date time.</returns>
        public DateTime? GetCalculatedNextExecutionDateTimeAsync(RecurssionDataEntity recurssionEntity)
        {
            _telemetry.TrackEvent("UpdateRecurssionDataNextExecutionDateTimeAsync");
            DateTime? calculatedNextExecutionDate = null;
            try
            {
                DateTime nextExecutionDate = Convert.ToDateTime(recurssionEntity.NextExecutionDate);
                RecurssionDataRepository recurssionDataRepository = new RecurssionDataRepository(_configuration, _telemetry);

                switch (recurssionEntity.RecursstionType.ToLower().Trim())
                {
                    case "daily":
                        DateTime? nextExecutionDay = DateTime.Now.AddDays(1);
                        calculatedNextExecutionDate = recurssionEntity.RecurssionEndDate >= nextExecutionDay ? nextExecutionDay : null;
                        break;
                    case "every weekday":
                        DateTime? nextWeekDay = GetNextWeekday();
                        calculatedNextExecutionDate = recurssionEntity.RecurssionEndDate >= nextWeekDay ? nextWeekDay : null;
                        break;
                    case "weekly":
                        DateTime? nextWeeklyday = GetNextWeeklyday(nextExecutionDate.DayOfWeek);
                        calculatedNextExecutionDate = recurssionEntity.RecurssionEndDate >= nextWeeklyday ? nextWeeklyday : null;
                        break;
                    case "monthly":
                        DateTime? nextMonthlyday = nextExecutionDate.AddMonths(1);
                        calculatedNextExecutionDate = recurssionEntity.RecurssionEndDate >= nextMonthlyday ? nextMonthlyday : null;
                        break;
                    case "does not repeat":
                        calculatedNextExecutionDate = null;
                        break;
                    case "custom":
                        if (recurssionEntity.CustomRecurssionTypeValue.Contains("week"))
                        {
                            var selectedweeks = new List<string>();
                            weekdays.ForEach(x =>
                            {
                                if (recurssionEntity.CustomRecurssionTypeValue.Contains(x))
                                {
                                    selectedweeks.Add(x);
                                }
                            });
                            if (selectedweeks.Count == 1)
                            {
                                goto case "weekly";
                            }
                            else if (selectedweeks.Count == 5 && selectedweeks.IndexOf("Saturday") == -1 && selectedweeks.IndexOf("Sunday") == -1)
                            {
                                goto case "every weekday";
                            }
                            else
                            {
                                var weekindex = selectedweeks.IndexOf(nextExecutionDate.DayOfWeek.ToString());
                                if ((weekindex + 1) < selectedweeks.Count)
                                {
                                    int addDays = weekdays.IndexOf(selectedweeks[weekindex + 1]) - weekdays.IndexOf(selectedweeks[weekindex]);
                                    DateTime? nextcustomweeklyday = DateTime.Now.AddDays(addDays);
                                    calculatedNextExecutionDate = recurssionEntity.RecurssionEndDate >= nextcustomweeklyday ? nextcustomweeklyday : null;
                                }
                            }

                            break;
                        }

                        if (recurssionEntity.CustomRecurssionTypeValue.Contains("month"))
                        {
                            if (recurssionEntity.CustomRecurssionTypeValue.Contains("Day"))
                            {
                                goto case "monthly";
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            DateTime? nextcustomdailyday = DateTime.Now.AddDays(1);
                            calculatedNextExecutionDate = recurssionEntity.RecurssionEndDate >= nextcustomdailyday ? nextcustomdailyday : null;
                            break;
                        }

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex);
            }
            return calculatedNextExecutionDate;
        }

        /// <summary>
        /// Add Reflection data in Table Storage.
        /// </summary>
        /// <param name="taskInfo">taskInfo.</param>
        /// <returns>A task that represents the work queued to execute.</returns>
        public async Task SaveReflectionFeedbackDataAsync(UserfeedbackInfo taskInfo)
        {
            _telemetry.TrackEvent("DeleteReflections");
            try
            {
                FeedbackDataRepository feedbackDataRepository = new FeedbackDataRepository(_configuration, _telemetry);

                if (taskInfo != null)
                {
                    var feedbackID = Guid.NewGuid();
                    var rowKey = Guid.NewGuid();

                    FeedbackDataEntity feedbackDataEntity = new FeedbackDataEntity
                    {
                        PartitionKey = PartitionKeyNames.FeedbackDataTable.FeedbackDataPartition,
                        RowKey = rowKey.ToString(),
                        FeedbackID = feedbackID,
                        FullName = taskInfo.userName,
                        ReflectionID = taskInfo.reflectionId,
                        FeedbackGivenBy = taskInfo.emailId,
                        Feedback = Convert.ToInt32(taskInfo.feedbackId)

                    };
                    await feedbackDataRepository.InsertOrMergeAsync(feedbackDataEntity);
                }
            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex);
            }
        }

        /// <summary>
        /// Gets user email id.
        /// </summary>
        /// <param name="turnContext">turnContext.</param>
        /// <returns>A task that represents the work queued to execute.</returns>
        public async Task<string> GetUserEmailId<T>(ITurnContext<T> turnContext) where T : Microsoft.Bot.Schema.IActivity
        {
            _telemetry.TrackEvent("GetUserEmailId");

            // Fetch the members in the current conversation
            try
            {
                IConnectorClient connector = turnContext.TurnState.Get<IConnectorClient>();

                var members = await connector.Conversations.GetConversationMembersAsync(turnContext.Activity.Conversation.Id);
                var user = AsTeamsChannelAccounts(members).FirstOrDefault(m => m.Id == turnContext.Activity.From.Id);
                return AsTeamsChannelAccounts(members).FirstOrDefault(m => m.Id == turnContext.Activity.From.Id).UserPrincipalName;
            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex);
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets view reflections data.
        /// </summary>
        /// <param name="reflectionId">reflectionId.</param>
        /// <returns>A task that represents the work queued to execute.</returns>
        public async Task<ViewReflectionsEntity> GetViewReflectionsData(Guid reflectionId)
        {
            _telemetry.TrackEvent("GetViewReflectionsData");

            try
            {
                ReflectionDataRepository reflectionDataRepository = new ReflectionDataRepository(_configuration, _telemetry);
                FeedbackDataRepository feedbackDataRepository = new FeedbackDataRepository(_configuration, _telemetry);
                ViewReflectionsEntity viewReflectionsEntity = new ViewReflectionsEntity();
                QuestionsDataRepository questionsDataRepository = new QuestionsDataRepository(_configuration, _telemetry);

                // Get reflection data
                ReflectionDataEntity refData = await reflectionDataRepository.GetReflectionData(reflectionId) ?? null;
                Dictionary<int, List<FeedbackDataEntity>> feedbackData = await feedbackDataRepository.GetReflectionFeedback(reflectionId) ?? null;
                List<QuestionsDataEntity> questions = await questionsDataRepository.GetQuestionsByQID(refData.QuestionID) ?? null;

                viewReflectionsEntity.ReflectionData = refData;
                viewReflectionsEntity.FeedbackData = feedbackData;
                viewReflectionsEntity.Question = questions.Find(x => x.QuestionID == refData.QuestionID);
                return viewReflectionsEntity;
            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex);
                return null;
            }
        }

        /// <summary>
        /// Get the data from the Recurrence able storage based on the email id.
        /// </summary>
        /// <param name="email">email id of the creator of the reflection.</param>
        /// <returns>RecurssionScreenData model.</returns>
        public async Task<List<RecurssionScreenData>> GetRecurrencePostsDataAsync(string email)
        {
            _telemetry.TrackEvent("GetRecurrencePostsDataAsync");
            try
            {
                ReflectionDataRepository reflectionDataRepository = new ReflectionDataRepository(_configuration, _telemetry);
                QuestionsDataRepository questionsDataRepository = new QuestionsDataRepository(_configuration, _telemetry);
                RecurssionDataRepository recurssionDataRepository = new RecurssionDataRepository(_configuration, _telemetry);
                List<ReflectionDataEntity> allActiveRefs = await reflectionDataRepository.GetAllActiveReflection(email);
                List<Guid?> allActiveRefIDs = allActiveRefs.Select(c => c.ReflectionID).ToList();
                List<Guid?> allActiveQuestionIDs = allActiveRefs.Select(c => c.QuestionID).ToList();
                List<QuestionsDataEntity> allQuestionsData = await questionsDataRepository.GetAllQuestionData(allActiveQuestionIDs);
                List<RecurssionDataEntity> allRecurssionData = await recurssionDataRepository.GetAllRecurssionData(allActiveRefIDs);
                List<RecurssionScreenData> screenData = new List<RecurssionScreenData>();
                foreach (var rec in allRecurssionData)
                {
                    RecurssionScreenData recurssionScreenData = new RecurssionScreenData();
                    recurssionScreenData.RefID = rec.ReflectionID;
                    var reflection = await reflectionDataRepository.GetReflectionData(rec.ReflectionID);
                    recurssionScreenData.CreatedBy = reflection.CreatedBy;
                    recurssionScreenData.RefCreatedDate = reflection.RefCreatedDate;
                    recurssionScreenData.Privacy = reflection.Privacy;
                    recurssionScreenData.Question = allQuestionsData.Where(c => c.QuestionID.ToString() == reflection.QuestionID.ToString()).Select(d => d.Question).FirstOrDefault().ToString();
                    recurssionScreenData.ExecutionDate = rec.ExecutionDate;
                    recurssionScreenData.ExecutionTime = rec.ExecutionTime;
                    recurssionScreenData.NextExecutionDate = rec.NextExecutionDate;
                    recurssionScreenData.RecurssionType = rec.RecursstionType;
                    recurssionScreenData.CustomRecurssionTypeValue = rec.CustomRecurssionTypeValue;
                    recurssionScreenData.ScheduleId = rec.ScheduleId;
                    if (recurssionScreenData.RecurssionType != null && recurssionScreenData.NextExecutionDate!=null)
                        screenData.Add(recurssionScreenData);
                }

                return screenData;
            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex);
                return null;
            }
        }

        /// <summary>
        /// Delete recurrence data based on the reflection id
        /// </summary>
        /// <param name="reflectionId">reflectionId.</param>
        /// <returns>Null.</returns>
        public async Task DeleteRecurrsionDataAsync(Guid reflectionId)
        {
            try
            {
                _telemetry.TrackEvent("DeleteRecurrsionDataAsync");
                ReflectionDataRepository reflectionDataRepository = new ReflectionDataRepository(_configuration, _telemetry);
                RecurssionDataRepository recurssionDataRepository = new RecurssionDataRepository(_configuration, _telemetry);
                var reflection = await reflectionDataRepository.GetReflectionData(reflectionId);
                var recurssion = await recurssionDataRepository.GetRecurssionData(reflection.RecurrsionID);
                await recurssionDataRepository.DeleteAsync(recurssion);
                await reflectionDataRepository.DeleteAsync(reflection);
            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex);
            }
        }

        /// <summary>
        /// Remove reflection id based in messageid
        /// </summary>
        /// <param name="reflectionMessageId">reflectionMessageId</param>
        /// <returns>messageid.</returns>
        public async Task<string> RemoveReflectionId(string reflectionMessageId)
        {
            string messageId = null;
            try
            {
                _telemetry.TrackEvent("RemoveMessageId");
                ReflectionDataRepository reflectionDataRepository = new ReflectionDataRepository(_configuration, _telemetry);
                FeedbackDataRepository feedbackDataRepository = new FeedbackDataRepository(_configuration, _telemetry);
                var reflection = await reflectionDataRepository.GetReflectionData(reflectionMessageId);
                messageId = reflection.MessageID;
                var feedbackCount = await feedbackDataRepository.GetFeedbackonRefId(reflection.ReflectionID);
                await feedbackDataRepository.DeleteAsync(feedbackCount);
                await reflectionDataRepository.DeleteAsync(reflection);

            }

            catch (Exception ex)
            {
                _telemetry.TrackException(ex);
            }

            return messageId;
        }

        /// <summary>
        /// Save Edit Recursion Data Async.
        /// </summary>
        /// <param name="reflection">reflection.</param>
        /// <returns>.</returns>
        public async Task SaveEditRecurssionDataAsync(RecurssionScreenData reflection)
        {
            try
            {
                _telemetry.TrackEvent("SaveEditRecurssionDataAsync");
                ReflectionDataRepository reflectionDataRepository = new ReflectionDataRepository(_configuration, _telemetry);
                RecurssionDataRepository recurssionDataRepository = new RecurssionDataRepository(_configuration, _telemetry);
                var reflectiondata = await reflectionDataRepository.GetReflectionData(reflection.RefID);
                var recurssion = await recurssionDataRepository.GetRecurssionData(reflectiondata.RecurrsionID);
                ReflectionDataEntity reflectionDataEntity = new ReflectionDataEntity();
                RecurssionDataEntity recurssionDataEntity = new RecurssionDataEntity();
                var reflectionid = Guid.NewGuid();
                var recurrsionid = Guid.NewGuid();
                reflectionDataEntity = reflectiondata;
                reflectionDataEntity.ReflectionID = reflectionid;
                reflectionDataEntity.RefCreatedDate = DateTime.Now;
                reflectionDataEntity.RecurrsionID = recurrsionid;
                reflectionDataEntity.RowKey = Guid.NewGuid().ToString();
                await reflectionDataRepository.CreateAsync(reflectionDataEntity);
                recurssionDataEntity = recurssion;
                recurssionDataEntity.ReflectionID = reflectionid;
                recurssionDataEntity.CreatedDate = DateTime.Now;
                recurssionDataEntity.RecurssionID = recurrsionid;
                recurssionDataEntity.RecursstionType = reflection.RecurssionType;
                recurssionDataEntity.CustomRecurssionTypeValue = reflection.CustomRecurssionTypeValue;
                recurssionDataEntity.RowKey = Guid.NewGuid().ToString();
                recurssionDataEntity.NextExecutionDate = reflection.NextExecutionDate;
                await recurssionDataRepository.CreateAsync(recurssionDataEntity);
                recurssion.NextExecutionDate = null;
                await recurssionDataRepository.CreateOrUpdateAsync(recurssion);
            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex);
            }
        }

        /// <summary>
        /// Encrypts the given text.
        /// </summary>
        /// <param name="text">text.</param>
        /// <returns>.</returns>
        public string Encrypt(string text)
        {
            var _key = Encoding.UTF8.GetBytes(_configuration["cipher"]);

            using (var aes = Aes.Create())
            {
                using (var encryptor = aes.CreateEncryptor(_key, aes.IV))
                {
                    using (var ms = new MemoryStream())
                    {
                        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                        {
                            using (var sw = new StreamWriter(cs))
                            {
                                sw.Write(text);
                            }
                        }

                        var iv = aes.IV;

                        var encrypted = ms.ToArray();

                        var result = new byte[iv.Length + encrypted.Length];

                        Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
                        Buffer.BlockCopy(encrypted, 0, result, iv.Length, encrypted.Length);

                        return Convert.ToBase64String(result);
                    }
                }
            }
        }

        /// <summary>
        /// Decrypts the encrypted text.
        /// </summary>
        /// <param name="encrypted">text.</param>
        /// <returns>.</returns>
        public string Decrypt(string encrypted)
        {
            var b = Convert.FromBase64String(encrypted);

            var iv = new byte[16];
            var cipher = new byte[16];

            Buffer.BlockCopy(b, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(b, iv.Length, cipher, 0, iv.Length);

            var _key = Encoding.UTF8.GetBytes(_configuration["cipher"]);

            using (var aes = Aes.Create())
            {
                using (var decryptor = aes.CreateDecryptor(_key, iv))
                {
                    var result = string.Empty;
                    using (var ms = new MemoryStream(cipher))
                    {
                        using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                        {
                            using (var sr = new StreamReader(cs))
                            {
                                result = sr.ReadToEnd();
                            }
                        }
                    }

                    return result;
                }
            }
        }

        /// <summary>
        /// Add Reflection data in Table Storage.
        /// </summary>
        /// <param name="channelAccountList">channelAccountList.</param>
        /// <returns>A task that represents the work queued to execute.</returns>
        private IEnumerable<TeamsChannelAccount> AsTeamsChannelAccounts(IEnumerable<ChannelAccount> channelAccountList)
        {
            _telemetry.TrackEvent("AsTeamsChannelAccounts");

            foreach (ChannelAccount channelAccount in channelAccountList)
            {
                yield return JObject.FromObject(channelAccount).ToObject<TeamsChannelAccount>();
            }
        }
    }
}