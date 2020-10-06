// -----------------------------------------------------------------------
// <copyright file="IDataBase.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Reflection.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder;
    using Reflection.Model;
    using Reflection.Repositories.RecursionData;
    using Reflection.Repositories.ReflectionData;

    /// <summary>
    /// DataBase interface.
    /// </summary>
    public interface IDataBase
    {
        Task SaveReflectionDataAsync(TaskInfo taskInfo);

        Task SaveQuestionsDataAsync(TaskInfo taskInfo);

        Task SaveDefaultQuestionsDataAsync();

        Task SaveRecursionDataAsync(TaskInfo taskInfo);

        Task SaveReflectionFeedbackDataAsync(UserfeedbackInfo taskInfo);

        Task SaveEditRecursionDataAsync(RecursionScreenData reflection);

        Task<ViewReflectionsEntity> GetViewReflectionsData(Guid reflectionId);

        Task<List<RecursionScreenData>> GetRecurrencePostsDataAsync(string email);

        Task UpdateReflectionMessageIdAsync(ReflectionDataEntity reflectionDataEntity);

        DateTime? GetCalculatedNextExecutionDateTimeAsync(RecursionDataEntity recurssionEntity);

        Task DeleteRecurrsionDataAsync(Guid reflectionId);

        Task<string> RemoveReflectionId(string reflectMessageId);

        Task<string> GetUserEmailId<T>(ITurnContext<T> turnContext) where T : Microsoft.Bot.Schema.IActivity;

        string Encrypt(string text);

        string Decrypt(string text);
    }
}