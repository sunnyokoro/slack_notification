using System;
using System.Collections.Generic;
using System.Text;

namespace CodeShare.SlaclNotify.Core.Models
{
    public class SendMessageRequest
    {
        public string Message { get; set; } //this should be the stack trace
        public string Type { get; set; } //error or message 
    }
}
