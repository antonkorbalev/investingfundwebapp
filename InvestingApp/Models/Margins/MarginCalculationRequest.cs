using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InvestingApp.Models.Margins
{
    public class MarginCalculationRequest
    {
        public string Instrument { get; set; }
        public StockExchange StockExchange { get; set; }
        public double Money { get; set; }
        public InstrumentType Type { get; set; }
    }
}