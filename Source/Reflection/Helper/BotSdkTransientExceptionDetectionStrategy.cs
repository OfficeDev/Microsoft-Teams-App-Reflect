// -----------------------------------------------------------------------
// <copyright file="BotSdkTransientExceptionDetectionStrategy.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Reflection.Helper
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;
    using Microsoft.Rest;

    /// <summary>
    /// Bot Sdk Transient Exception Detection Strategy.
    /// </summary>
    public class BotSdkTransientExceptionDetectionStrategy : ITransientErrorDetectionStrategy
    {
        /// <summary>
        /// List of error codes to retry on.
        /// </summary>
        private readonly List<int> transientErrorStatusCodes = new List<int>() { 429 };

        /// <summary>
        /// Get user feedback.
        /// </summary>
        /// <param name="ex">ex.</param>
        /// <returns>Boolean.</returns>
        public bool IsTransient(Exception ex)
        {
            if (ex.Message.Contains("429"))
            {
                return true;
            }

            var httpOperationException = ex as HttpOperationException;
            if (httpOperationException != null)
            {
                return httpOperationException.Response != null &&
                        transientErrorStatusCodes.Contains((int)httpOperationException.Response.StatusCode);
            }

            return false;
        }
    }
}
