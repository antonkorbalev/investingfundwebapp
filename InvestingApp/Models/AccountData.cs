using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InvestingApp.Database.Entities;
using InvestingApp.Helpers;

namespace InvestingApp.Models
{
    public class AccountData
    {
        public string UserName { get; }
        public double Benefit { get; }
        public double SharedRatio { get; }
        public IEnumerable<FlowRow> Flows { get; }
        public double TotalProfitPercent { get; }
        public double Money { get; }
        public double OthersMoney { get; }
        public double TotalProfit { get; }
        public double LastMonthProfit { get; }
        public double LastMonthPercent { get; }

        public Dictionary<DateTime, double> Balances { get; }
        public IEnumerable<ProfitPerPeriod> ProfitsPerMonth { get; private set; }

        public AccountData(User user, 
            IEnumerable<BalancesRow> balances, 
            IEnumerable<FlowRow> flows)
        {
            UserName = user.Name;
            Flows = flows.Where(o => o.User.Id == user.Id).ToArray();
            var startDate = Flows.Min(o => o.DateTimeStamp);

            var ratios = new Dictionary<DateTime, double>();
            var dbs = balances.OrderBy(o => o.DateTimeStamp).ToDictionary(o => o.DateTimeStamp, o => o.Balance);
            Balances = new Dictionary<DateTime, double>();

            double ownMoney = 0;
            double othersMoney = 0;
            DateTime date = DateTime.MinValue;

            foreach (var d in dbs.Keys)
            {
                var currFlows = flows.Where(o => o.DateTimeStamp <= d && o.DateTimeStamp > date);
                var currBal = dbs[d] - currFlows.Sum(o => o.Payment);
                var profit = d == dbs.Min(o => o.Key) ? 0 : currBal - (othersMoney + ownMoney);
                var currRatio = d == dbs.Min(o => o.Key) ? 0 : ownMoney / (othersMoney + ownMoney);
                ownMoney += profit * currRatio;
                othersMoney += profit * (1 - currRatio);

                foreach (var f in currFlows)
                    if (f.User.Id == user.Id)
                        ownMoney += f.Payment;
                    else
                        othersMoney += f.Payment;
                ratios.Add(d, ownMoney / (othersMoney + ownMoney));
                if (startDate <= d)
                    Balances.Add(d, ownMoney);
                date = d;
            }

            var ownFlowsSum = Flows.Sum(o => o.Payment);
            SharedRatio = Math.Round(ratios[balances.Last().DateTimeStamp] * 100, 2);
            Money = Math.Round(ownMoney, 2);
            OthersMoney = Math.Round(othersMoney, 2);

            var lastMonthBalance = dbs.LastOrDefault(o => o.Key < date && o.Key.Month != date.Month);
            var lastMonthInOut = Flows.Where(o => o.DateTimeStamp > lastMonthBalance.Key).Sum(o => o.Payment);
            LastMonthProfit = Math.Round(Money - lastMonthBalance.Value * ratios[lastMonthBalance.Key] - lastMonthInOut , 2);

            TotalProfit = Math.Round(Money - ownFlowsSum, 2);
            if (TotalProfit > 0)
                Money -= Math.Round(TotalProfit * (Benefit / 100), 2);
            if (LastMonthProfit > 0)
                LastMonthProfit -= Math.Round(LastMonthProfit * (Benefit / 100), 2);

            LastMonthPercent = Math.Round(100 * LastMonthProfit / (lastMonthBalance.Value * ratios[lastMonthBalance.Key]), 2);

            TotalProfitPercent = Math.Round(100 * TotalProfit / ownFlowsSum, 2);

            ProfitsPerMonth = StatsCalculator.GetProfitsByMonths(StatsCalculator.GetProfits(Balances, Flows), Balances);
        }
    }
}