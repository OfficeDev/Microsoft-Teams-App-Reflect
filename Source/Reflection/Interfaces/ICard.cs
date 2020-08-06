// -----------------------------------------------------------------------
// <copyright file="ICard.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Reflection.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Threading.Tasks;
    using AdaptiveCards;
    using Reflection.Model;
    using Reflection.Repositories.FeedbackData;

    /// <summary>
    /// Card interface.
    /// </summary>
    public interface ICard
    {
        AdaptiveCard FeedBackCard(Dictionary<int, List<FeedbackDataEntity>> keyValues, Guid? reflectionId, string questionName);

        Task<string> saveImage(Image data, string Filepath);

        AdaptiveCard CreateNewReflect(TaskInfo data);

        AdaptiveCard ConfirmationCard(string messageId);
    }
}