using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CodeShare.SlackNotify.Extensions;
using CodeShare.SlackNotify.Helpers;
using CodeShare.SlackNotify.Infrastructure;
using CodeShare.SlaclNotify.Core.Entities;
using CodeShare.SlaclNotify.Core.Interfaces;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.Swagger;

namespace CodeShare.SlackNotify
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; set; }
         
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            //.AddJsonFile($"appsettings.{environment}.json", true, true)
            .AddEnvironmentVariables();

            Configuration = builder.Build();

            //mock
            services.AddDbContext<Database>(options => options.UseInMemoryDatabase(databaseName: "Database"));
            services.AddSingleton<IConfiguration>(Configuration); 
            //services.AddHttpClient<ISlackMessageSender, SlackMessageSender>();
            services.AddHttpClient<ISlackMessageSender, SlackMessageSender>(c =>
            {
                c.BaseAddress = new Uri("https://hooks.slack.com");
            });
            services.AddHangfire(c => c.UseMemoryStorage());//inmemory

            //swagger 
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "CodeShare.Slack Notify", Version = "v1" } );
            });

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //to allow access on server. Currently set to allow all
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new HangfireAuthorizationFilter() }
            });
            app.UseHangfireServer();

            //custom error log middleware extention
            app.UseErrorLogging();


            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Bank APIs for FinTechs (v1)");
            });
        }
    }
}
