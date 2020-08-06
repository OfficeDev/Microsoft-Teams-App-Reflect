// -----------------------------------------------------------------------
// <copyright file="ReflectionDataEntity.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Reflection.Repositories.ReflectionData
{
    using System;
    using Microsoft.Azure.Cosmos.Table;

    /// <summary>
    /// ReflectionDataEntity.
    /// </summary>
    public class ReflectionDataEntity : TableEntity
    {
        /// <summary>
        /// Gets or sets reflection Id.
        /// </summary>
        public Guid? ReflectionID { get; set; }

        /// <summary>
        /// Gets or sets name Id as created by.
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets email Id as created by.
        /// </summary>
        public string CreatedByEmail { get; set; }

        /// <summary>
        /// Gets or sets Reflection creation date.
        /// </summary>
        public DateTime RefCreatedDate { get; set; }

        /// <summary>
        /// Gets or sets question.
        /// </summary>
        public Guid? QuestionID { get; set; }

        /// <summary>
        /// Gets or sets privacy.
        /// </summary>
        public string Privacy { get; set; }

        /// <summary>
        /// Gets or sets recurring ID.
        /// </summary>
        public Guid? RecurrsionID { get; set; }

        /// <summary>
        /// Gets or sets Message ID.
        /// </summary>
        public string MessageID { get; set; }


        /// <summary>
        /// Gets or sets Send now flag.
        /// </summary>
        public bool? SendNowFlag { get; set; }

        /// <summary>
        /// Gets or sets Channel ID.
        /// </summary>
        public string ChannelID { get; set; }

        /// <summary>
        /// Gets or sets active flag.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets Reflect Message Id.
        /// </summary>
        public string ReflectMessageId { get; set; }

        /// <summary>
        /// Gets or sets TenantId.
        /// </summary>
        public string TenantId { get; set; }

        /// <summary>
        /// Gets or sets ServiceUrl.
        /// </summary>
        public string ServiceUrl { get; set; }

        /// <summary>
        /// Gets or sets Schedule Id.
        /// </summary>
        public string ScheduleId { get; set; }
    }
}
