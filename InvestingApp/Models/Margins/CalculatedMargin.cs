using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InvestingApp.Models.Margins
{
    public class CalculatedMargin
    {
        public MarginRow[] Margins
        {
            get
            {
                var ms = new List<MarginRow>();
                for (var i = 90; i >= 50; i -= 5)
                {
                    var am = Math.Floor((i * Money / 100) / InitialMargin);
                    var m = Math.Round(am * InitialMargin, 2);
                    ms.Add(new MarginRow() {
                        Amount = am,
                        Money = m,
                        Percent = i,
                        Residue = Math.Round(Money - m, 2)
                    });
                }
                return ms.ToArray();
            }
        }

        public DateTime DeliveryDate { get; set; }
        public string InstrumentName { get; set; }
        public string InstrumentCode { get; set; }
        public double InitialMarginPercent { get; set; }
        public double InitialMargin { get; set; }
        public double Money { get; set; }

        public bool Success { get; set; }
    }
}