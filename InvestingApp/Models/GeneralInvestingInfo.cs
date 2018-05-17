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
            if ((flows == null)
                || (Flows.Where(o => o.DateTimeStamp <= data.First().DateTimeStamp)
                .Sum(o => o.Payment) != data.First().Balance))
                    throw new InvalidOperationException("Error in database! Flows sum is not equal to initial balance.");
            Profits = (from d in Data
                       select
                       new
                       {
                           Balance = d.Balance - (Flows != null ? Flows
                                 .Where(f => f.DateTimeStamp <= d.DateTimeStamp)
                                 .Sum(o => o.Payment) : 0),
                           DateTimeStamp = d.DateTimeStamp
                       }).ToDictionary(o => o.DateTimeStamp, o => o.Balance);
            updateProfitsPerMonth();
        }

        private void updateProfitsPerMonth()
        {
            var start = Profits.Keys.First();
            var distr = new List<ProfitPerPeriod>();

            while (true)
            {
                var stop = Profits.Last(o => o.Key >= start && o.Key.Month == start.Month);
                var startBal = Profits.First(o => o.Key == start).Value;
                var currProfit = stop.Value - startBal;
                distr.Add(
                    new ProfitPerPeriod(stop.Key.ToString("MMM yy", CultureInfo.InvariantCulture ),
                    currProfit,
                    startBal != 0 ?  Math.Round(100 * currProfit / startBal, 2) : 0
                    ));
                if (Data.Any(o => o.DateTimeStamp > stop.Key))
                    start = Data.First(o => o.DateTimeStamp > stop.Key).DateTimeStamp;
                else break;
            }

            ProfitsPerMonth = distr.ToArray();
        }

        public Dictionary<DateTime, double> Profits { get; set; }

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
                return Profits.Values.Last();
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
                    (from d in Profits
                    let prevs = Profits.TakeWhile(o => o.Key != d.Key)
                    let max = prevs.Any() ? prevs.Max(o => o.Value) : 0
                    select (max - d.Value)).Max();
            }
        }

        /// <summary>
        /// Last Month Profit
        /// </summary>
        public double LastMonthProfit
        {
            get
            {
                var today = Profits.Last().Key;
                var firstMonthProfitDate = Profits.Keys
                    .First(o => o.Year == today.Year && o.Month == today.Month);
                return Profits[today] - Profits[firstMonthProfitDate];
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
                var firstMonthProfitDate = Profits.Keys
                    .First(o => o.Year == today.Year && o.Month == today.Month);
                return Math.Round(100 * LastMonthProfit / Profits[firstMonthProfitDate], 2);
            }
        }
    }
}