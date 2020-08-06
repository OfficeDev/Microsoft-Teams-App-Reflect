// -----------------------------------------------------------------------
// <copyright file="ViewReflectionsEntity.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Reflection.Model
{
    using System.Collections.Generic;
    using Reflection.Repositories.FeedbackData;
    using Reflection.Repositories.QuestionsData;
    using Reflection.Repositories.ReflectionData;

      /// <summary>
      /// User class
      /// </summary>
    public class ViewReflectionsEntity
    {
        /// <summary>
        /// Gets or sets ReflectionData.
        /// </summary>
        public ReflectionDataEntity ReflectionData { get; set; }

        /// <summary>
        /// Gets or sets FeedbackData.
        /// </summary>
        public Dictionary<int, List<FeedbackDataEntity>> FeedbackData { get; set; }

        /// <summary>
        /// Gets or sets Question.
        /// </summary>
        public QuestionsDataEntity Question { get; set; }
    }
}
