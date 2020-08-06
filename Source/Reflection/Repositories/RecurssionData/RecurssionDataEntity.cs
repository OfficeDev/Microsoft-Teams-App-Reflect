// -----------------------------------------------------------------------
// <copyright file="RecurssionDataEntity.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Reflection.Repositories.RecurssionData
{
    using System;
    using Microsoft.Azure.Cosmos.Table;

    /// <summary>
    /// RecurssionData Entity.
    /// </summary>
    public class RecurssionDataEntity : TableEntity
    {
        /// <summary>
        /// Gets or sets RecurssionID.
        /// </summary>
        public Guid? RecurssionID { get; set; }

        /// <summary>
        /// Gets or sets ReflectionID.
        /// </summary>
        public Guid? ReflectionID { get; set; }

        /// <summary>
        /// Gets or sets RecursstionType.
        /// </summary>
        public string RecursstionType { get; set; }

        /// <summary>
        /// Gets or sets CreatedDate.
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets ExecutionDate.
        /// </summary>
        public DateTime? ExecutionDate { get; set; }

        /// <summary>
        /// Gets or sets ExecutionTime.
        /// </summary>
        public string ExecutionTime { get; set; }

        /// <summary>
        /// Gets or sets RecurssionEndDate.
        /// </summary>
        public DateTime? RecurssionEndDate { get; set; }

        /// <summary>
        /// Gets or sets NextExecutionDate.
        /// </summary>
        public DateTime? NextExecutionDate { get; set; }

        /// <summary>
        /// Gets or sets ReflectionRowKey.
        /// </summary>
        public string ReflectionRowKey { get; set; }

        /// <summary>
        /// Gets or sets QuestionRowKey.
        /// </summary>
        public string QuestionRowKey { get; set; }

        /// <summary>
        /// Gets or sets CustomRecurssionTypeValue.
        /// </summary>
        public string CustomRecurssionTypeValue { get; set; }

        /// <summary>
        /// Gets or sets Schedule Id.
        /// </summary>
        public string ScheduleId { get; set; }

    }
}
