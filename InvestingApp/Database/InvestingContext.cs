using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.Migrations;

namespace InvestingApp.Database
{
    public class InvestingContext : DbContext
    {
        public DbSet<Entities.BalancesRow> Balances { get; set; }
        public DbSet<Entities.FlowRow> Flows { get; set; }
        public DbSet<Entities.User> Users { get; set; }
        public DbSet<Entities.Rate> Rates { get; set; }
        public InvestingContext() : base("cstr")
        {
            
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("public");           
            base.OnModelCreating(modelBuilder);
        }

    }
}