// -----------------------------------------------------------------------
// <copyright file="ReflectionDataEntity.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Reflection.Repositories.ReflectionData
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.ApplicationInsights;
    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// ReflectionData Repository.
    /// </summary>
    public class ReflectionDataRepository : BaseRepository<ReflectionDataEntity>
    {
        private TelemetryClient _telemetry;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReflectionDataRepository"/> class.
        /// </summary>
        /// <param name="configuration">Represents the application configuration.</param>
        /// <param name="telemetry">telemetry.</param>
        /// <param name="isFromAzureFunction">Flag to show if created from Azure Function.</param>
        public ReflectionDataRepository(IConfiguration configuration, TelemetryClient telemetry, bool isFromAzureFunction = false)
            : base(
                configuration,
                PartitionKeyNames.ReflectionDataTable.TableName,
                PartitionKeyNames.ReflectionDataTable.ReflectionDataPartition,
                isFromAzureFunction)
        {
            _telemetry = telemetry;
        }

        /// <summary>
        /// Get Reflection Data.
        /// </summary>
        /// <param name="refID">refID.</param>
        /// <returns>ReflectionDataEntity.</returns>
        public async Task<ReflectionDataEntity> GetReflectionData(Guid? refID)
        {
            _telemetry.TrackEvent("GetReflectionData");
            try
            {
                var allReflections = await this.GetAllAsync(PartitionKeyNames.ReflectionDataTable.TableName);
                ReflectionDataEntity refData = allReflections.Where(c => c.ReflectionID == refID).FirstOrDefault();
                return refData;
            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex);
                return null;
            }
        }

        /// <summary>
        /// Get Reflection Data.
        /// </summary>
        /// <param name="reflectMessagId">reflectMessagId.</param>
        /// <returns>ReflectionDataEntity.</returns>
        public async Task<ReflectionDataEntity> GetReflectionData(string reflectMessagId)
        {
            _telemetry.TrackEvent("GetReflectionData");
            try
            {
                var allReflections = await this.GetAllAsync(PartitionKeyNames.ReflectionDataTable.TableName);
                ReflectionDataEntity refData = allReflections.Where(c => c.ReflectMessageId == reflectMessagId).FirstOrDefault();
                return refData;
            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex);
                return null;
            }
        }

        /// <summary>
        /// Get message Id from Reflection.
        /// </summary>
        /// <param name="refId">refId.</param>
        /// <returns>string.</returns>
        public async Task<string> GetmessageIdfromReflection(Guid refId)
        {
            _telemetry.TrackEvent("GetmessageIdfromReflection");
            try
            {
                var allRefs = await this.GetAllAsync(PartitionKeyNames.ReflectionDataTable.TableName);
                var dataEntity = allRefs.Where(c => c.ReflectionID == refId).FirstOrDefault();
                return dataEntity.MessageID;
            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex);
                return null;
            }
        }

        /// <summary>
        /// Get All Active Reflection.
        /// </summary>
        /// <param name="email">email.</param>
        /// <returns>ReflectionDataEntity.</returns>
        public async Task<List<ReflectionDataEntity>> GetAllActiveReflection(string email)
        {
            _telemetry.TrackEvent("GetAllActiveReflection");
            try
            {
                var allRefs = await this.GetAllAsync(PartitionKeyNames.ReflectionDataTable.TableName);
                List<ReflectionDataEntity> refDataEntity = allRefs.Where(c => c.IsActive == true && c.CreatedByEmail == email).ToList();
                return refDataEntity;
            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex);
                return null;
            }
        }
    }
}
