using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InvestingApp.Database;
using InvestingApp.Database.Entities;

namespace InvestingApp.Models
{
    public class GeneralInvestingInfo
    {
        public IEnumerable<BalancesRow> Data { get; set; }

        /// <summary>
        /// Total profit
        /// </summary>
        public double Profit
        {
            get
            {
                return Data.Last().Balance - Data.First().Balance;
            }
        }

        /// <summary>
        /// Percent of total profit
        /// </summary>
        public double ProfitPercent
        {
            get
            {
                return Math.Round(100 * Profit / Data.First().Balance ,2);
            }
        }

        /// <summary>
        /// Max balance
        /// </summary>
        public double Max
        {
            get
            {
                return Data.Max(o => o.Balance);
            }
        }

        /// <summary>
        /// Max balance
        /// </summary>
        public double Min
        {
            get
            {
                return Data.Min(o => o.Balance);
            }
        }

        /// <summary>
        /// DrawDown value
        /// </summary>
        public double DrawDown
        {
            get
            {
                return
                    (from d in Data
                    let prevs = Data.TakeWhile(o => o != d)
                    let max = prevs.Any() ? prevs.Max(o => o.Balance) : 0
                    select (max - d.Balance)).Max();
            }
        }
    }
}