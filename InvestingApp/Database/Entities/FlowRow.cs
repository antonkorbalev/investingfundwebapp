﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace InvestingApp.Database.Entities
{
    public class FlowRow : BaseEntity
    {
        [Required]
        public User User { get; set; }
        [Required]
        public DateTime DateTimeStamp { get; set; }
        [Required]
        public double Payment { get; set; }
        public string Description { get; set; }
    }
}