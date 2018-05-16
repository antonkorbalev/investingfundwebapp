using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InvestingApp.Database;
using InvestingApp.Database.Entities;
using System.Globalization;

namespace InvestingApp.Models
{
    public class GeneralInvestingInfo
    {

        public GeneralInvestingInfo(IEnumerable<BalancesRow> data, IEnumerable<FlowRow> flows)
        {
            Data = data;
            Flows = flows;
            updateProfitsPerMonth();
        }

        private void updateProfitsPerMonth()
        {
            var start = Data.First().DateTimeStamp;
            var distr = new List<ProfitPerPeriod>();

            while (true)
            {
                var stop = Data.Last(o => o.DateTimeStamp >= start && o.DateTimeStamp.Month == start.Month);
                var startBal = Data.First(o => o.DateTimeStamp == start).Balance;
                var currProfit = stop.Balance - startBal;
                distr.Add(
                    new ProfitPerPeriod(stop.DateTimeStamp.ToString("MMM yy", CultureInfo.InvariantCulture ),
                    currProfit,
                    startBal != 0 ?  Math.Round(100 * currProfit / startBal, 2) : 0
                    ));
                if (Data.Any(o => o.DateTimeStamp > stop.DateTimeStamp))
                    start = Data.First(o => o.DateTimeStamp > stop.DateTimeStamp).DateTimeStamp;
                else break;
            }

            ProfitsPerMonth = distr.ToArray();
        }


        public ProfitPerPeriod[] ProfitsPerMonth { get; private set; }

        /// <summary>
        /// Balances rows
        /// </summary>
        public IEnumerable<BalancesRow> Data { get; private set; }
        
        /// <summary>
        /// Flows rows
        /// </summary>
        public IEnumerable<FlowRow> Flows { get; private set; }

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
                return Math.Round(100 * Profit / Data.First().Balance, 2);
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

        /// <summary>
        /// Last Month Profit
        /// </summary>
        public double LastMonthProfit
        {
            get
            {
                var today = Data.Last().DateTimeStamp;
                return Data.Last().Balance - Data.First(o => o.DateTimeStamp.Year == today.Year && o.DateTimeStamp.Month == today.Month).Balance;
            }
        }

        /// <summary>
        /// Last month profit percent
        /// </summary>
        public double LastMonthProfitPercent
        {
            get
            {
                var today = Data.Last().DateTimeStamp;
                return Math.Round(100 * LastMonthProfit / Data.First(o => o.DateTimeStamp.Year == today.Year && o.DateTimeStamp.Month == today.Month).Balance, 2);
            }
        }
    }
}