// -----------------------------------------------------------------------
// <copyright file="QuestionsDataRepository.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Reflection.Repositories.QuestionsData
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.ApplicationInsights;
    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// QuestionsData Repository.
    /// </summary>
    public class QuestionsDataRepository : BaseRepository<QuestionsDataEntity>
    {
        private TelemetryClient _telemetry;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuestionsDataRepository"/> class.
        /// </summary>
        /// <param name="configuration">Represents the application configuration.</param>
        /// <param name="telemetry">telemetry.</param>
        /// <param name="isFromAzureFunction">Flag to show if created from Azure Function.</param>
        public QuestionsDataRepository(IConfiguration configuration, TelemetryClient telemetry, bool isFromAzureFunction = false)
            : base(
                configuration,
                PartitionKeyNames.QuestionsDataTable.TableName,
                PartitionKeyNames.QuestionsDataTable.QuestionsDataPartition,
                isFromAzureFunction)
        {
            _telemetry = telemetry;
        }

        /// <summary>
        /// Get the default questions.
        /// </summary>
        /// <param name="userEmail">userEmail.</param>
        /// <returns>Questions which have default flag true.</returns>
        public async Task<List<QuestionsDataEntity>> GetAllDefaultQuestionsForUser(string userEmail)
        {
            _telemetry.TrackEvent("GetAllDefaultQuestionsForUser");

            try
            {
                var allRows = await this.GetAllAsync(PartitionKeyNames.QuestionsDataTable.TableName);
                var result = allRows.Where(d => d.IsDefaultFlag == true || d.CreatedByEmail == userEmail);
                return result.ToList();
            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex);
                return null;
            }
        }

        /// <summary>
        /// Get Questions By QID.
        /// </summary>
        /// <param name="qID">qID.</param>
        /// <returns>QuestionsDataEntity.</returns>
        public async Task<List<QuestionsDataEntity>> GetQuestionsByQID(Guid? qID)
        {
            _telemetry.TrackEvent("GetQuestionsByQID");

            try
            {
                var allRows = await this.GetAllAsync(PartitionKeyNames.QuestionsDataTable.TableName);
                var result = allRows.Where(d => d.IsDefaultFlag == true || d.QuestionID == qID);
                return result.ToList();
            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex);
                return null;
            }
        }

        /// <summary>
        /// Get All Question Data.
        /// </summary>
        /// <param name="quesID">quesID.</param>
        /// <returns>QuestionsDataEntity.</returns>
        public async Task<List<QuestionsDataEntity>> GetAllQuestionData(List<Guid?> quesID)
        {
            _telemetry.TrackEvent("GetAllQuestionData");
            try
            {
                var allRows = await this.GetAllAsync(PartitionKeyNames.QuestionsDataTable.TableName);
                List<QuestionsDataEntity> result = allRows.Where(c => quesID.Contains(c.QuestionID)).ToList();
                return result ?? null;
            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex);
                return null;
            }
        }

        /// <summary>
        /// Is Question Already Present.
        /// </summary>
        /// <param name="question">question.</param>
        /// <param name="email">email.</param>
        /// <returns>Bool.</returns>
        public async Task<bool> IsQuestionAlreadyPresent(string question, string email)
        {
            _telemetry.TrackEvent("IsQuestionAlreadyPresent");
            try
            {
                var allRows = await this.GetAllAsync(PartitionKeyNames.QuestionsDataTable.TableName);
                var result = allRows.Where(c => c.Question == question);

                if (result.Any(c => c.IsDefaultFlag == true || c.CreatedByEmail == email))
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex);
                return false;
            }
        }

        /// <summary>
        /// Get Question Data.
        /// </summary>
        /// <param name="quesID">quesID.</param>
        /// <returns>Bool.</returns>
        public async Task<QuestionsDataEntity> GetQuestionData(Guid? quesID)
        {
            _telemetry.TrackEvent("GetQuestionData");
            try
            {
                var allRows = await this.GetAllAsync(PartitionKeyNames.QuestionsDataTable.TableName);
                QuestionsDataEntity result = allRows.Where(c => c.QuestionID == quesID).FirstOrDefault();
                return result ?? null;
            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex);
                return null;
            }
        }
    }
}
