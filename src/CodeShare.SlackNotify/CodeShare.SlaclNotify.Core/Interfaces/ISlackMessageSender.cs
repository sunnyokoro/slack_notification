using CodeShare.SlaclNotify.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CodeShare.SlaclNotify.Core.Interfaces
{
    public interface ISlackMessageSender
    {
        Task<Response<string>> SendMessageAsync(SendMessageRequest model);
        Task<Response<string>> SendMessageHttpClientAsync(SendMessageRequest model);

        
    }
}
