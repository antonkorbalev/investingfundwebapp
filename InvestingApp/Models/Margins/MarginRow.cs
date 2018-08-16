using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InvestingApp.Models.Margins
{
    public class MarginRow
    {
        public double Percent { get; set; }
        public double Money { get; set; }
        public double Amount { get; set; }
        public double Residue { get; set; }
    }
}