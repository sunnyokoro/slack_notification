using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CodeShare.SlaclNotify.Core.Entities
{
    public class Database : DbContext
    {
        public Database(DbContextOptions<Database> options)
           : base(options)
        {
            //add custom tables 
        }

        public DbSet<SlackMessage> SlackMessages { get; set; }
    }
}
