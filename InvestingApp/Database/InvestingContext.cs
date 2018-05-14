﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace InvestingApp.Database
{
    public class InvestingContext : DbContext
    {
        public DbSet<Entities.BalancesRow> Balances { get; set; }

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