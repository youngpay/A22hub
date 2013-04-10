using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace A2208hub.Store.Web.Models
{
    public class StoreDBContext : DbContext
    {
        public StoreDBContext()
            : base("name=DefaultConnection")
        { }

        public DbSet<FileLog> FileLogs { get; set; }
        public DbSet<FileLink> FileLinks { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}