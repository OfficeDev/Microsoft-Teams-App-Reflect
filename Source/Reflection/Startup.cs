// -----------------------------------------------------------------------
// <copyright file="Startup.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
//      Licensed under the MIT License.
//      Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.6.2
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Teams.Apps.Reflect.Web
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Integration.AspNet.Core;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Reflection.Helper;
    using Reflection.Interfaces;
    using Reflection.Repositories.QuestionsData;
    using Reflection.Repositories.ReflectionData;

    /// <summary>
    /// Startup class.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// Renders index view.
        /// </summary>
        /// <param name="configuration">configuration.</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddMvc();

            // Create the Bot Framework Adapter with error handling enabled.
            services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();
            services.AddSingleton<QuestionsDataRepository>();
            services.AddSingleton<ReflectionDataRepository>();
            services.AddSingleton<ICard, CardHelper>();
            services.AddSingleton<IDataBase, DBHelper>();
            services.AddMemoryCache(); // Add this line
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            // Create the bot as a transient. In this case the ASP Controller is expecting an IBot.
            services.AddTransient<IBot, MessageExtension>();
            services.AddApplicationInsightsTelemetry();
            services.AddHostedService<SchedulerHelper>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }


            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseWebSockets();

            // Runs matching. An endpoint is selected and set on the HttpContext if a match is found.
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                // Mapping of endpoints goes here:
                endpoints.MapControllers();
                endpoints.MapControllerRoute(
                   name: "default",
                   pattern: "{controller=Home}/{action=Index}/{id?}");

            });

            //app.UseHttpsRedirection();

        }
    }
}
