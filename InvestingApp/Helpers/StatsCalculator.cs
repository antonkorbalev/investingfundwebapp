using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InvestingApp.Database.Entities;
using InvestingApp.Models;
using System.Globalization;

namespace InvestingApp.Helpers
{
    public static class StatsCalculator
    {
        public static Dictionary<DateTime, double> GetProfits(IEnumerable<BalancesRow> balances,
            IEnumerable<FlowRow> flows)
        {
            return (from d in balances
                    select
                    new
                    {
                        Balance = d.Balance - (flows != null ? flows
                              .Where(f => f.DateTimeStamp <= d.DateTimeStamp && f.Payment > 0)
                              .Sum(o => o.Payment) : 0),
                        DateTimeStamp = d.DateTimeStamp
                    }).ToDictionary(o => o.DateTimeStamp, o => o.Balance);
        }

        public static Dictionary<DateTime, double> GetProfits(Dictionary<DateTime, double> balances,
            IEnumerable<FlowRow> flows)
        {
            return GetProfits(balances.Select(o => new BalancesRow()
            {
                DateTimeStamp = o.Key,
                Balance = o.Value
            }), flows);
        }

        public static IEnumerable<ProfitPerPeriod> GetProfitsByMonths(Dictionary<DateTime, double> profits,
            IEnumerable<BalancesRow> balances)
        {
            var start = profits.First();
            var distr = new List<ProfitPerPeriod>();

            var stop = profits.Last(o => o.Key >= start.Key && o.Key.Month == start.Key.Month);

            while (stop.Key <= profits.Last().Key)
            {
                var currProfit = stop.Value - start.Value;
                distr.Add(
                    new ProfitPerPeriod(stop.Key.ToString("MMM yy", CultureInfo.InvariantCulture),
                    Math.Round(currProfit, 2),
                    Math.Round(100 * currProfit / balances.First(o => o.DateTimeStamp == start.Key).Balance, 2)
                    ));

                start = stop;
                if (profits.Any(o => o.Key > start.Key))
                {
                    var nextMonth = profits.First(o => o.Key > start.Key).Key.Month;
                    stop = profits.Last(o => o.Key.Month == nextMonth && o.Key > start.Key);
                }
                else
                    break;
            }

            return distr;
        }

        public static IEnumerable<ProfitPerPeriod> GetProfitsByMonths(Dictionary<DateTime, double> profits,
        Dictionary<DateTime, double> balances)
        {
            return GetProfitsByMonths(profits, 
                balances.Select(o => new BalancesRow()
                {
                    DateTimeStamp = o.Key,
                    Balance = o.Value
                }));
        }

        public static double CalculateSharpeRatio(IEnumerable<BalancesRow> bals,
            IEnumerable<FlowRow> flows,
            double riskFreeRate)
        {
            var returns = new List<double>();
            var dates = bals.Select(o => o.DateTimeStamp).ToArray();
            var balances = bals.Select(o => o.Balance).ToArray();

            for (var i = 1; i < bals.Count(); i++)
            {
                var riskFreeDayPercent = riskFreeRate / (DateTime.IsLeapYear(dates[i].Year) ? 366 : 365) / 100;
                var dayPayments = flows.Where(o => o.DateTimeStamp == dates[i]).Sum(o => o.Payment);
                var totalDayPercent = ((balances[i] - balances[i - 1] - dayPayments) / balances[i - 1]);
                returns.Add(totalDayPercent - riskFreeDayPercent);
            }

            var average = returns.Any() ? returns.Average() : 0;
            var sumOfSquares = returns.Sum(o => (o - average) * (o - average));
            double sd = Math.Sqrt(sumOfSquares / (returns.Count() - 1));
            return Math.Round(average / sd, 2);
        }

        public static IEnumerable<double> GetDollarBHs(IEnumerable<Rate> ratesUSD,
            Dictionary<DateTime, double> profits,
            IEnumerable<FlowRow> flows
            )
        {
            var dollarBHs = new double[0];
            if (ratesUSD == null || !ratesUSD.Any())
                return new double[0];

            var bhs = new List<double>();
            var ratesDic = ratesUSD.ToDictionary(o => o.DateTimeStamp, o => o.Value);

            double currRate = ratesDic[ratesUSD.Min(o => o.DateTimeStamp)];
            double sumUSD = 0;
            var datesList = profits.Keys.ToList();
            datesList.AddRange(flows.Where(o => o.Payment > 0).Select(o => o.DateTimeStamp));

            foreach (var d in datesList.Distinct().OrderBy(o => o))
            {
                if (ratesDic.ContainsKey(d))
                    currRate = ratesDic[d];

                foreach (var f in flows.Where(o => o.DateTimeStamp == d && o.Payment > 0))
                    sumUSD += f.Payment / currRate;

                if (profits.ContainsKey(d))
                    bhs.Add(Math.Round(sumUSD * currRate, 2));
            }

            return bhs;
        }

        public static IEnumerable<double> GetSavingsPerMonth(IEnumerable<BalancesRow> data,
            IEnumerable<FlowRow> flows,
            double riskFreeRate)
        {
            double bal = 0;
            var savings = new List<double>();
            var firstDate = data.First().DateTimeStamp.Date;
            var flowDates = flows.Where(o => o.Payment > 0)
                .Select(o => o.DateTimeStamp.Date);

            bal += flows.Where(o => o.DateTimeStamp < data.First().DateTimeStamp && o.Payment > 0)
                .Select(o => o.Payment).Sum();

            DateTime prevDate = firstDate;
            foreach (var d in data.Select(o => o.DateTimeStamp.Date))
            {
                if (flowDates.Contains(d))
                    bal += flows.Where(o => o.DateTimeStamp.Date == d && o.Payment > 0)
                        .Sum(o => o.Payment);
                if (d.Month != prevDate.Month)
                {
                    var daysDiff = (d - firstDate).TotalDays < DateTime.DaysInMonth(d.Year, d.Month) ?
                        (d - firstDate).TotalDays : DateTime.DaysInMonth(d.Year, d.Month);

                   var periodStartDate = data
                        .First(o => o.DateTimeStamp.Month == d.Month && o.DateTimeStamp.Year == d.Year)
                        .DateTimeStamp;

                   bal += (bal - flows.Where(o => o.Payment > 0 && o.DateTimeStamp >= periodStartDate && o.DateTimeStamp < d).Sum(o => o.Payment))
                        * daysDiff * 0.01 * (riskFreeRate) / (365 + (DateTime.IsLeapYear(d.Year) ? 1 : 0));
                }
                savings.Add(bal);
                prevDate = d;
            }

            return savings;
        }
    }
}