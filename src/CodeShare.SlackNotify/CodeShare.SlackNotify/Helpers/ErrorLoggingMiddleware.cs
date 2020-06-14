using CodeShare.SlaclNotify.Core.Entities;
using CodeShare.SlaclNotify.Core.Services;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeShare.SlackNotify.Helpers
{
    public class ErrorLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceProvider _serviceProvider;
        private IBackgroundJobClient _job;
        private Database _db;

        public ErrorLoggingMiddleware(RequestDelegate next
            , IServiceProvider serviceProvider)
        {
            _next = next;
            _serviceProvider = serviceProvider;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }

            catch (Exception ex)
            {
                using (var serviceScope = _serviceProvider.CreateScope())
                {
                    var scopeServiceProvider = serviceScope.ServiceProvider;
                    _db = scopeServiceProvider.GetRequiredService<Database>();
                    _job = scopeServiceProvider.GetRequiredService<IBackgroundJobClient>();

                    System.Diagnostics.Debug.WriteLine($"The following error happened: {ex.Message}");
                    
                    //save record to db and use hangfire to process the stuff 
                    var message = new SlackMessage
                    {
                        Message = ex.Message,
                        StackTrace = ex.StackTrace,
                        Type = SlaclNotify.Core.Constants.SLACK_TYPE_ERROR
                    };
                    _db.SlackMessages.Add(message);
                    await _db.SaveChangesAsync();

                    _job.Enqueue<SlackMessageService>(x => x.Execute(message.Id));
                }

                throw; //Throwing the exception makes sure that other pieces of middleware handling exceptions still work.
            }
        }
    }
}
