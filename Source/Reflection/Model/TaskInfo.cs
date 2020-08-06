// -----------------------------------------------------------------------
// <copyright file="TaskInfo.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Reflection.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Newtonsoft.Json;

    /// <summary>
    /// TaskInfo.
    /// </summary>
    public class TaskInfo
    {
        /// <summary>
        /// Gets or sets question.
        /// </summary>
        public string question { get; set; }

        /// <summary>
        /// Gets or sets privacy.
        /// </summary>
        public string privacy { get; set; }

        /// <summary>
        /// Gets or sets postCreateBy.
        /// </summary>
        public string postCreateBy { get; set; }

        /// <summary>
        /// Gets or sets postCreatedByEmail.
        /// </summary>
        public string postCreatedByEmail { get; set; }

        /// <summary>
        /// Gets or sets executionDate.
        /// </summary>
        public DateTime executionDate { get; set; }

        /// <summary>
        /// Gets or sets executionTime.
        /// </summary>
        public string executionTime { get; set; }

        /// <summary>
        /// Gets or sets nextExecutionDate.
        /// </summary>
        public DateTime? nextExecutionDate { get; set; }

        /// <summary>
        /// Gets or sets postDate.
        /// </summary>
        public DateTime? postDate { get; set; }

        /// <summary>
        /// Gets or sets postSendNowFlag.
        /// </summary>
        [DefaultValue(false)]
        public bool postSendNowFlag { get; set; }

        /// <summary>
        /// Gets or sets repeatFrequency.
        /// </summary>
        public string repeatFrequency { get; set; }

        /// <summary>
        /// Gets or sets recurssionType.
        /// </summary>
        public string recurssionType { get; set; }

        /// <summary>
        /// Gets or sets customRecurssionTypeValue.
        /// </summary>
        public string customRecurssionTypeValue { get; set; }

        /// <summary>
        /// Gets or sets IsActive.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets action.
        /// </summary>
        public string action { get; set; }

        /// <summary>
        /// Gets or sets card.
        /// </summary>
        public int? card { get; set; }

        /// <summary>
        /// Gets or sets userResponce.
        /// </summary>
        public int? userResponce { get; set; }

        /// <summary>
        /// Gets or sets messageID.
        /// </summary>
        public string messageID { get; set; }

        /// <summary>
        /// Gets or sets isDelete.
        /// </summary>
        public bool isDelete { get; set; }
        

        /// <summary>
        /// Gets or sets channelID.
        /// </summary>
        public string channelID { get; set; }

        /// <summary>
        /// Gets or sets reflectionID.
        /// </summary>
        public Guid? reflectionID { get; set; }

        /// <summary>
        /// Gets or sets questionID.
        /// </summary>
        public Guid? questionID { get; set; }

        /// <summary>
        /// Gets or sets recurssionID.
        /// </summary>
        public Guid? recurssionID { get; set; }

        /// <summary>
        /// Gets or sets isDefaultQuestion.
        /// </summary>
        public bool isDefaultQuestion { get; set; }

        /// <summary>
        /// Gets or sets reflectionRowKey.
        /// </summary>
        public string reflectionRowKey { get; set; }

        /// <summary>
        /// Gets or sets recurrsionRowKey.
        /// </summary>
        public string recurrsionRowKey { get; set; }

        /// <summary>
        /// Gets or sets questionRowKey.
        /// </summary>
        public string questionRowKey { get; set; }

        /// <summary>
        /// Gets or sets type.
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// Gets or sets reflectMessageId.
        /// </summary>
        public string reflectMessageId { get; set; }

        /// <summary>
        /// Gets or sets teantId.
        /// </summary>
        public string teantId { get; set; }

        /// <summary>
        /// Gets or sets serviceUrl.
        /// </summary>
        public string serviceUrl { get; set; }

        /// <summary>
        /// Gets or sets feedback.
        /// </summary>
        public int feedback { get; set; }

        /// <summary>
        /// Gets or sets Schedule Id.
        /// </summary>
        public string scheduleId { get; set; }
    }

    /// <summary>
    /// UserfeedbackInfo.
    /// </summary>
    public class UserfeedbackInfo
    {
        /// <summary>
        /// Gets or sets feedbackId.
        /// </summary>
        public int feedbackId { get; set; }

        /// <summary>
        /// Gets or sets reflectionId.
        /// </summary>
        public Guid? reflectionId { get; set; }

        /// <summary>
        /// Gets or sets action.
        /// </summary>
        public string action { get; set; }

        /// <summary>
        /// Gets or sets type.
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// Gets or sets userName.
        /// </summary>
        public string userName { get; set; }

        /// <summary>
        /// Gets or sets emailId.
        /// </summary>
        public string emailId { get; set; }

        /// <summary>
        /// Gets or sets messageId.
        /// </summary>
        public string messageId { get; set; }


    }

    /// <summary>
    /// MessageDetails.
    /// </summary>
    public class MessageDetails
    {
        /// <summary>
        /// Gets or sets messageid.
        /// </summary>
        public string messageid { get; set; }
    }

    /// <summary>
    /// ReflctionData.
    /// </summary>
    public class ReflctionData
    {
        /// <summary>
        /// Gets or sets data.
        /// </summary>
        public Data data { get; set; }

        /// <summary>
        /// Gets or sets datajson.
        /// </summary>
        public DataJson datajson { get; set; }
    }

    /// <summary>
    /// Data.
    /// </summary>
    public class Data
    {
        /// <summary>
        /// Gets or sets URL.
        /// </summary>
        public string URL { get; set; }

        /// <summary>
        /// Gets or sets type.
        /// </summary>
        public string type { get; set; }
    }

    /// <summary>
    /// Data Json.
    /// </summary>
    public class DataJson
    {
        /// <summary>
        /// Gets or sets ReflectionId.
        /// </summary>
        public Guid? ReflectionId { get; set; }

        /// <summary>
        /// Gets or sets FeedbackId.
        /// </summary>
        public int FeedbackId { get; set; }
    }

    /// <summary>
    /// Question Test.
    /// </summary>
    public class QuestionTest
    {
        /// <summary>
        /// Gets or sets Question.
        /// </summary>
        public string Question { get; set; }
    }

    /// <summary>
    /// TaskModuleActionHelper.
    /// </summary>
    public class TaskModuleActionHelper
    {
        /// <summary>
        /// AdaptiveCardValue.
        /// </summary>
        public class AdaptiveCardValue<T>
        {
            /// <summary>
            /// Gets or sets Type.
            /// </summary>
            [JsonProperty("msteams")]
            public object Type { get; set; } = JsonConvert.DeserializeObject("{\"type\": \"task/fetch\" }");

            /// <summary>
            /// Gets or sets Data.
            /// </summary>
            [JsonProperty("data")]
            public T Data { get; set; }

            /// <summary>
            /// Gets or sets DataJson.
            /// </summary>
            [JsonProperty("datajson")]
            public DataJson DataJson { get; set; }
        }
    }

    /// <summary>
    /// ActionDetails.
    /// </summary>
    public class ActionDetails
    {
        /// <summary>
        /// Gets or sets type.
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// Gets or sets ActionType.
        /// </summary>
        public string ActionType { get; set; }
    }

    /// <summary>
    /// TaskModuleActionDetails.
    /// </summary>
    public class TaskModuleActionDetails : ActionDetails
    {
        /// <summary>
        /// Gets or sets URL.
        /// </summary>
        public string URL { get; set; }

        /// <summary>
        /// Gets or sets Title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets Width.
        /// </summary>
        public string Width { get; set; }

        /// <summary>
        /// Gets or sets Height.
        /// </summary>
        public string Height { get; set; }
    }

    /// <summary>
    /// Responses.
    /// </summary>
    public class Responses
    {
        /// <summary>
        /// Gets or sets Createdby.
        /// </summary>
        public string Createdby { get; set; }

        /// <summary>
        /// Gets or sets QuestionTitle.
        /// </summary>
        public string QuestionTitle { get; set; }

        /// <summary>
        /// Gets or sets OptionResponses.
        /// </summary>
        public List<OptionResponse> OptionResponses { get; set; }
    }

    /// <summary>
    /// Option Response.
    /// </summary>
    public class OptionResponse
    {
        /// <summary>
        /// Gets or sets Width.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets Color.
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// Gets or sets Description.
        /// </summary>
        public string Description { get; set; }
    }

    /// <summary>
    /// ReflectConstants.
    /// </summary>
    public static class ReflectConstants
    {
        public static readonly string SaveFeedBack = "saveFeedback";
        public static readonly string RemovePosts = "removeposts";
        public static readonly string RecurringPosts = "recurringreflections";
        public static readonly string CreateReflect = "createreflect";
    }
}
