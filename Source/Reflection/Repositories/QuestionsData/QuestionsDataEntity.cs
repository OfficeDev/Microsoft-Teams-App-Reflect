// -----------------------------------------------------------------------
// <copyright file="QuestionsDataEntity.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Reflection.Repositories.QuestionsData
{
    using System;
    using Microsoft.Azure.Cosmos.Table;

    /// <summary>
    /// QuestionsDataEntity.
    /// </summary>
    public class QuestionsDataEntity : TableEntity
    {
        /// <summary>
        /// Gets or sets QuestionID.
        /// </summary>
        public Guid? QuestionID { get; set; }

        /// <summary>
        /// Gets or sets Question.
        /// </summary>
        public string Question { get; set; }

        /// <summary>
        /// Gets or sets QuestionCreatedDate.
        /// </summary>
        public DateTime QuestionCreatedDate { get; set; }

        /// <summary>
        /// Gets or sets IsDefaultFlag.
        /// </summary>
        public bool IsDefaultFlag { get; set; }

        /// <summary>
        /// Gets or sets CreatedBy.
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets CreatedByEmail.
        /// </summary>
        public string CreatedByEmail { get; set; }

    }
}
