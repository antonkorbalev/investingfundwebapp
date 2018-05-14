using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InvestingApp.Models
{
    public class ProfitPerPeriod
    {
        public string Description { get; set; }
        public double Value { get; set; }
        public double Percent { get; set; }

        public ProfitPerPeriod(string description, double value, double percent)
        {
            Description = description;
            Value = value;
            Percent = percent;
        }
    }
}