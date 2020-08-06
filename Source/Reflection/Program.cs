// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
//      Licensed under the MIT License.
//      Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.6.2
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Teams.Apps.Reflect.Web
{
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;

    /// <summary>
    /// Program starting class.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main Class.
        /// </summary>
        /// <param name="args">This parameter is a main program arguments.</param>
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// CreateWebHostBuilder method.
        /// </summary>
        /// <param name="args">This parameter is a main program arguments.</param>
        /// <returns>returns start file</returns>
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
