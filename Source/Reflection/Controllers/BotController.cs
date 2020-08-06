// -----------------------------------------------------------------------
// <copyright file="BotController.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.6.2
// -----------------------------------------------------------------------

namespace Microsoft.Teams.Apps.Reflect.Web.Controllers
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Integration.AspNet.Core;

    /// <summary>
    /// Bot controller.
    /// </summary>
    [Route("api/messages")]
    [ApiController]
    public class BotController : ControllerBase
    {
        private readonly IBotFrameworkHttpAdapter Adapter;
        private readonly IBot Bot;
        private readonly TelemetryClient _telemetry;

        /// <summary>
        /// Initializes a new instance of the <see cref="BotController"/> class.
        /// Bot controller.
        /// </summary>
        public BotController(IBotFrameworkHttpAdapter adapter, IBot bot, TelemetryClient telemetry)
        {
            Adapter = adapter;
            Bot = bot;
            _telemetry = telemetry;
        }

        /// <summary>
        /// Task post async.
        /// </summary>
        [HttpPost]
        public async Task PostAsync()
        {
            _telemetry.TrackEvent("PostAsync");
            try
            {
                await Adapter.ProcessAsync(Request, Response, Bot);
            }
            catch (Exception ex)
            {
                _telemetry.TrackException(ex);
            }
        }
    }
}
