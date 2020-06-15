using CodeShare.SlaclNotify.Core.Entities;
using CodeShare.SlaclNotify.Core.Interfaces;
using CodeShare.SlaclNotify.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CodeShare.SlaclNotify.Core.Services
{
    public class SlackMessageService
    {
        private readonly Database _db;
        private readonly HttpClient _httpClient;
        private readonly ISlackMessageSender _slackmessager;
 

        public SlackMessageService(Database db, 
            ISlackMessageSender slackmessager
             )
        {
            _db = db;
            _slackmessager = slackmessager; 
        }

        public async Task Execute(int messageId)
        {
            var message = _db.SlackMessages.FirstOrDefault(x =>  x.Id == messageId && !x.SentOn.HasValue);

            if (message == null)
                return;

            //var response = await _slackmessager.SendMessageAsync(new SendMessageRequest
            var response = await _slackmessager.SendMessageHttpClientAsync(new SendMessageRequest
            {
                Message = message.StackTrace,
                Type = message.Type
            });

            if (response.Success)
            {
                message.SentOn = DateTime.Now;
            }

            _db.SaveChanges();
        }
    }
}
