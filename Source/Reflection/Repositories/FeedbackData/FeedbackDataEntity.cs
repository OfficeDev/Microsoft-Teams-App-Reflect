// -----------------------------------------------------------------------
// <copyright file="FeedbackDataEntity.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Reflection.Repositories.FeedbackData
{
    using System;
    using Microsoft.Azure.Cosmos.Table;

    /// <summary>
    /// Feedback DataEntity.
    /// </summary>
    public class FeedbackDataEntity : TableEntity
    {
        /// <summary>
        /// Gets or sets Feedback Id.
        /// </summary>
        public Guid FeedbackID { get; set; }

        /// <summary>
        /// Gets or sets full name.
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Gets or sets reflection Id.
        /// </summary>
        public Guid? ReflectionID { get; set; }

        /// <summary>
        /// Gets or sets FeedbackGivenBy.
        /// </summary>
        public string FeedbackGivenBy { get; set; }

        /// <summary>
        /// Gets or sets Feedback.
        /// </summary>
        public int Feedback { get; set; }
    }
}
