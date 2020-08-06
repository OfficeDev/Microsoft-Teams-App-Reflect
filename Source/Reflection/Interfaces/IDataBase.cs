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
    using Reflection.Repositories.RecurssionData;
    using Reflection.Repositories.ReflectionData;

    /// <summary>
    /// DataBase interface.
    /// </summary>
    public interface IDataBase
    {
        Task SaveReflectionDataAsync(TaskInfo taskInfo);

        Task SaveQuestionsDataAsync(TaskInfo taskInfo);

        Task SaveRecurssionDataAsync(TaskInfo taskInfo);

        Task SaveReflectionFeedbackDataAsync(UserfeedbackInfo taskInfo);

        Task SaveEditRecurssionDataAsync(RecurssionScreenData reflection);

        Task<ViewReflectionsEntity> GetViewReflectionsData(Guid reflectionId);

        Task<List<RecurssionScreenData>> GetRecurrencePostsDataAsync(string email);

        Task UpdateReflectionMessageIdAsync(ReflectionDataEntity reflectionDataEntity);

        DateTime? GetCalculatedNextExecutionDateTimeAsync(RecurssionDataEntity recurssionEntity);

        Task DeleteRecurrsionDataAsync(Guid reflectionId);

        Task<string> RemoveReflectionId(string reflectMessageId);

        Task<string> GetUserEmailId<T>(ITurnContext<T> turnContext) where T : Microsoft.Bot.Schema.IActivity;

        string Encrypt(string text);

        string Decrypt(string text);
    }
}