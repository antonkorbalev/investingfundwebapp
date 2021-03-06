﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace InvestingApp.Database.Entities
{
    public class BalancesRow : BaseEntity
    {
        [Required]
        public DateTime DateTimeStamp { get; set; }
        [Required]
        public double Balance { get; set; }
    }
}