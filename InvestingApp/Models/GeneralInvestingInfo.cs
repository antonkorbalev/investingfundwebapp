using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InvestingApp.Database;
using InvestingApp.Database.Entities;
using System.Globalization;
using System.Threading.Tasks;

namespace InvestingApp.Models
{
    public class GeneralInvestingInfo
    {

        public GeneralInvestingInfo(IEnumerable<BalancesRow> data, IEnumerable<FlowRow> flows, IEnumerable<Rate> ratesUSD = null)
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
            updateSavingsPerMonth();
            UpdateDollarBHs(ratesUSD);
        }

        private void updateProfitsPerMonth()
        {
            var start = Profits.First();
            var distr = new List<ProfitPerPeriod>();

            var stop = Profits.Last(o => o.Key >= start.Key && o.Key.Month == start.Key.Month);
            
            while (stop.Key <= Profits.Last().Key)
            {
                var currProfit = stop.Value - start.Value;
                distr.Add(
                    new ProfitPerPeriod(stop.Key.ToString("MMM yy", CultureInfo.InvariantCulture),
                    Math.Round(currProfit, 2),
                    Math.Round(100 * currProfit / Data.First(o => o.DateTimeStamp == start.Key).Balance, 2)
                    ));

                start = stop;
                if (Profits.Any(o => o.Key > start.Key))
                {
                    var nextMonth = Profits.First(o => o.Key > start.Key).Key.Month;
                    stop = Profits.Last(o => o.Key.Month == nextMonth && o.Key > start.Key);
                }
                else
                    break;
            }

            ProfitsPerMonth = distr.ToArray();
        }

        private void updateSavingsPerMonth()
        {
            double bal = 0;
            double rate = 5;
            var sl = new List<double>();
            var firstDate = Data.First().DateTimeStamp.Date;
            var flowDates = Flows.Where(o => o.Payment > 0)
                .Select(o => o.DateTimeStamp.Date);

            bal += Flows.Where(o => o.DateTimeStamp < Data.First().DateTimeStamp && o.Payment > 0)
                .Select(o => o.Payment).Sum();

            DateTime prevDate = firstDate;
            foreach (var d in Data.Select(o => o.DateTimeStamp.Date))
            {
                if (flowDates.Contains(d))
                    bal += Flows.Where(o => o.DateTimeStamp.Date == d && o.Payment > 0)
                        .Sum(o => o.Payment);
                if (d.Month != prevDate.Month)
                {
                    var daysDiff = (d - firstDate).TotalDays < DateTime.DaysInMonth(d.Year, d.Month) ? 
                        (d - firstDate).TotalDays : DateTime.DaysInMonth(d.Year, d.Month);
                    bal += bal * daysDiff * 0.01 * (rate) / (365 + (DateTime.IsLeapYear(d.Year) ? 1 : 0));
                }
                sl.Add(bal);
                prevDate = d;
            }

            Savings = sl.ToArray();
        }

        private void UpdateDollarBHs(IEnumerable<Rate> ratesUSD)
        {
            DollarBHs = new double[0];
            if (ratesUSD == null || !ratesUSD.Any())
                return;

            var bhs = new List<double>();
            var ratesDic = ratesUSD.ToDictionary(o => o.DateTimeStamp, o => o.Value);

            double currRate = ratesDic[ratesUSD.Min(o => o.DateTimeStamp)];
            double sumUSD = 0;
            var datesList = Profits.Keys.ToList();
            datesList.AddRange(Flows.Where(o => o.Payment > 0).Select(o => o.DateTimeStamp));

            foreach (var d in datesList.Distinct().OrderBy(o => o))
            {
                if (ratesDic.ContainsKey(d))
                    currRate = ratesDic[d];

                foreach (var f in Flows.Where(o => o.DateTimeStamp == d && o.Payment > 0))
                    sumUSD += f.Payment / currRate;

                if (Profits.ContainsKey(d))
                    bhs.Add(Math.Round(sumUSD * currRate, 2));
            }

            DollarBHs = bhs.ToArray();
        }

        public Dictionary<DateTime, double> Profits { get; private set; }

        public double[] Savings { get; private set; }

        public double[] DollarBHs { get; private set; }

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
                    (from d in Profits.Skip(1)
                    let prevs = Profits.TakeWhile(o => o.Key != d.Key)
                    let max =  prevs.Max(o => o.Value)
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
                return ProfitsPerMonth.Last().Value;
            }
        }

        /// <summary>
        /// Last month profit percent
        /// </summary>
        public double LastMonthProfitPercent
        {
            get
            {
                return ProfitsPerMonth.Last().Percent;
            }
        }
    }
}