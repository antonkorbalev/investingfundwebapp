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

        private double getStartBalance(long fromTicks = 0, long toTicks = long.MaxValue)
        {
            var dataBalance = Data.Where(o => o.DateTimeStamp.Ticks >= fromTicks).First().Balance;
            return (Flows != null && Flows.Any()) ?
                Flows.Where(o => o.DateTimeStamp.Ticks >= fromTicks && o.DateTimeStamp.Ticks <= toTicks)
                .Sum(o => o.Payment) + dataBalance
                : dataBalance;
        }

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
                var currProfit = stop.Balance - getStartBalance(start.Ticks, stop.DateTimeStamp.Ticks);
                var startBal = getStartBalance(start.Ticks, stop.DateTimeStamp.Ticks);
                var bal = getStartBalance(start.Ticks, stop.DateTimeStamp.Ticks);
                distr.Add(
                    new ProfitPerPeriod(stop.DateTimeStamp.ToString("MMMM yy"),
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
                return Data.Last().Balance - getStartBalance();
            }
        }

        /// <summary>
        /// Percent of total profit
        /// </summary>
        public double ProfitPercent
        {
            get
            {
                return Math.Round(100 * Profit / getStartBalance(), 2);
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
                return Data.Last().Balance - getStartBalance(Data.Max(o => o.DateTimeStamp).AddMonths(-1).Ticks);
            }
        }

        /// <summary>
        /// Last month profit percent
        /// </summary>
        public double LastMonthProfitPercent
        {
            get
            {
                return Math.Round(100 * Profit / getStartBalance(Data.Max(o => o.DateTimeStamp).AddMonths(-1).Ticks), 2);
            }
        }
    }
}