using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CodeShare.SlaclNotify.Core.Entities
{
    public class SlackMessage   
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { set; get; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public string Type { get; set; } //error or msg
        public DateTime? SentOn { get; set; }
    }
}
