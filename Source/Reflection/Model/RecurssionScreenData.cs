// -----------------------------------------------------------------------
// <copyright file="RecurssionScreenData.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Reflection.Model
{
    using System;

    /// <summary>
    /// RecurssionScreenData.
    /// </summary>
    public class RecurssionScreenData
    {
        /// <summary>
        /// Gets or sets RefID.
        /// </summary>
        public Guid? RefID { get; set; }

        /// <summary>
        /// Gets or sets CreatedBy.
        /// </summary>
        public string  CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets RefCreatedDate.
        /// </summary>
        public DateTime RefCreatedDate { get; set; }

        /// <summary>
        /// Gets or sets Privacy.
        /// </summary>
        public string Privacy { get; set; }

        /// <summary>
        /// Gets or sets Question.
        /// </summary>
        public string Question { get; set; }

        /// <summary>
        /// Gets or sets ExecutionDate.
        /// </summary>
        public DateTime? ExecutionDate { get; set; }

        /// <summary>
        /// Gets or sets ExecutionTime.
        /// </summary>
        public string ExecutionTime { get; set; }

        /// <summary>
        /// Gets or sets RecurssionType.
        /// </summary>
        public string RecurssionType { get; set; }

        /// <summary>
        /// Gets or sets CustomRecurssionTypeValue.
        /// </summary>
        public string CustomRecurssionTypeValue { get; set; }

        /// <summary>
        /// Gets or sets ExecutionTime.
        /// </summary>
        public DateTime? NextExecutionDate { get; set; }

        /// <summary>
        /// Gets or sets ScheduleId.
        /// </summary>
        public string ScheduleId { get; set; }
    }
}
