using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using InvestingApp.Models.Rates;

namespace InvestingApp.Database.Entities
{
    public class Rate : BaseEntity
    {
        [Required]
        public RateType Type { get; set; }

        [Required]
        public DateTime DateTimeStamp { get; set; }
        [Required]
        public double Value { get; set; }
    }
}