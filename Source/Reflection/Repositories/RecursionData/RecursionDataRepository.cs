// -----------------------------------------------------------------------
// <copyright file="RecursionDataRepository.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Reflection.Repositories.RecursionData
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.ApplicationInsights;
    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// RecursionData Repository.
    /// </summary>
    public class RecursionDataRepository : BaseRepository<RecursionDataEntity>
    {
        private TelemetryClient _telemetry;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecursionDataRepository"/> class.
        /// </summary>
        /// <param name="configuration">Represents the application configuration.</param>
        /// <param name="telemetry">telemetry.</param>
        /// <param name="isFromAzureFunction">Flag to show if created from Azure Function.</param>
        public RecursionDataRepository(IConfiguration configuration, TelemetryClient telemetry, bool isFromAzureFunction = false)
            : base(
                configuration,
                PartitionKeyNames.RecursionDataTable.TableName,
                PartitionKeyNames.RecursionDataTable.RecursionDataPartition,
                isFromAzureFunction)
        {
            _telemetry = telemetry;
        }

        /// <summary>
        /// Get All Recursion Data.
        /// </summary>
        /// <param name="refIds">refIds.</param>
        /// <returns>RecursionDataEntity.</returns>
        public async Task<List<RecursionDataEntity>> GetAllRecursionData(List<Guid?> refIds)
        {
            _telemetry.TrackEvent("GetAllRecursionData");

            try
            {
                var allRows = await this.GetAllAsync(PartitionKeyNames.RecursionDataTable.TableName);
                List<RecursionDataEntity> result = allRows.Where(c => refIds.Contains(c.ReflectionID) && c.RecursstionType != "Does not repeat").ToList();
                return result;
            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex);
                return null;
            }
        }

        /// <summary>
        /// Get Recursion Data.
        /// </summary>
        /// <param name="recurssionId">recurssionId.</param>
        /// <returns>RecursionDataEntity.</returns>
        public async Task<RecursionDataEntity> GetRecursionData(Guid? recurssionId)
        {
            _telemetry.TrackEvent("GetRecursionData");
            try
            {
                var allRows = await this.GetAllAsync(PartitionKeyNames.RecursionDataTable.TableName);
                RecursionDataEntity result = allRows.Where(c => c.RecursionID == recurssionId).FirstOrDefault();
                return result;
            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex);
                return null;
            }
        }

        /// <summary>
        /// Get All Recursion Data.
        /// </summary>
        /// <returns>RecursionDataEntity.</returns>
        public async Task<List<RecursionDataEntity>> GetAllRecursionData()
        {
            DateTime dateTime = DateTime.UtcNow;
            dateTime = dateTime.AddSeconds(-dateTime.Second);
            dateTime = dateTime.AddMilliseconds(-dateTime.Millisecond);
            _telemetry.TrackEvent("GetAllRecursionData");
            try
            {
                var recurssionData = await this.GetAllAsync(PartitionKeyNames.RecursionDataTable.TableName);
                var recData = recurssionData.Where(c => c.NextExecutionDate != null).ToList();
                var intervalRecords = recData.Where(r => dateTime.Subtract((DateTime)r.NextExecutionDate).TotalSeconds < 60 && dateTime.Subtract((DateTime)r.NextExecutionDate).TotalSeconds > 0).ToList();
                return intervalRecords;
            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex);
                return null;
            }
        }
    }
}