// -----------------------------------------------------------------------
// <copyright file="RecurssionDataRepository.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Reflection.Repositories.RecurssionData
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.ApplicationInsights;
    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// RecurssionData Repository.
    /// </summary>
    public class RecurssionDataRepository : BaseRepository<RecurssionDataEntity>
    {
        private TelemetryClient _telemetry;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecurssionDataRepository"/> class.
        /// </summary>
        /// <param name="configuration">Represents the application configuration.</param>
        /// <param name="telemetry">telemetry.</param>
        /// <param name="isFromAzureFunction">Flag to show if created from Azure Function.</param>
        public RecurssionDataRepository(IConfiguration configuration, TelemetryClient telemetry, bool isFromAzureFunction = false)
            : base(
                configuration,
                PartitionKeyNames.RecurssionDataTable.TableName,
                PartitionKeyNames.RecurssionDataTable.RecurssionDataPartition,
                isFromAzureFunction)
        {
            _telemetry = telemetry;
        }

        /// <summary>
        /// Get All Recurssion Data.
        /// </summary>
        /// <param name="refIds">refIds.</param>
        /// <returns>RecurssionDataEntity.</returns>
        public async Task<List<RecurssionDataEntity>> GetAllRecurssionData(List<Guid?> refIds)
        {
            _telemetry.TrackEvent("GetAllRecurssionData");

            try
            {
                var allRows = await this.GetAllAsync(PartitionKeyNames.RecurssionDataTable.TableName);
                List<RecurssionDataEntity> result = allRows.Where(c => refIds.Contains(c.ReflectionID) && c.RecursstionType != "Does not repeat").ToList();
                return result;
            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex);
                return null;
            }
        }

        /// <summary>
        /// Get Recurssion Data.
        /// </summary>
        /// <param name="recurssionId">recurssionId.</param>
        /// <returns>RecurssionDataEntity.</returns>
        public async Task<RecurssionDataEntity> GetRecurssionData(Guid? recurssionId)
        {
            _telemetry.TrackEvent("GetRecurssionData");
            try
            {
                var allRows = await this.GetAllAsync(PartitionKeyNames.RecurssionDataTable.TableName);
                RecurssionDataEntity result = allRows.Where(c => c.RecurssionID == recurssionId).FirstOrDefault();
                return result;
            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex);
                return null;
            }
        }

        /// <summary>
        /// Get All Recurssion Data.
        /// </summary>
        /// <returns>RecurssionDataEntity.</returns>
        public async Task<List<RecurssionDataEntity>> GetAllRecurssionData()
        {
            DateTime dateTime = DateTime.UtcNow;
            dateTime = dateTime.AddSeconds(-dateTime.Second);
            dateTime = dateTime.AddMilliseconds(-dateTime.Millisecond);
            _telemetry.TrackEvent("GetAllRecurssionData");
            try
            {
                var recurssionData = await this.GetAllAsync(PartitionKeyNames.RecurssionDataTable.TableName);
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