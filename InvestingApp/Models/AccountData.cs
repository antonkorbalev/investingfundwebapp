using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InvestingApp.Database.Entities;

namespace InvestingApp.Models
{
    public class AccountData
    {
        public string UserName { get; set; }
        public double Benefit { get; set; }
        public double SharedRatio { get; set; }
        public IEnumerable<FlowRow> Flows { get; set; }
        public IEnumerable<BalancesRow> Balances { get; set; }

        private double getPersonalValue(double value)
        {
            return Math.Round(value * (SharedRatio - Benefit) / 100, 2);
        }

        public double Money
        {
            get
            {
                return getPersonalValue(Balances.Last().Balance);
            }
        }

        public double OthersMoney
        {
            get
            {
                return Balances.Last().Balance - Money;
            }
        }

        public double LastDayProfit
        {
            get
            {
                var lastBal = Balances.Last();
                var dayBal = Balances.Last(o => o.DateTimeStamp < lastBal.DateTimeStamp);
                var value = lastBal.Balance - dayBal.Balance;
                LastDayProfitPercent = Math.Round((value / dayBal.Balance) * 100, 2);
                return getPersonalValue(value);
            }
        }

        public double LastDayProfitPercent { get; private set; }
        public double LastMonthProfit
        {
            get
            {
                var lastBal = Balances.Last();
                var lastMonthBal = Balances.Last();
                for (var i = Balances.Count() - 1; i >= 0; i--)
                {
                    lastMonthBal = Balances.ElementAt(i);
                    if ((lastMonthBal.DateTimeStamp.Month != lastBal.DateTimeStamp.Month))
                        break;
                }
                var value = lastBal.Balance - lastMonthBal.Balance;
                LastMonthProfitPercent = Math.Round((value / lastMonthBal.Balance) * 100, 2);
                return getPersonalValue(value);
            }
        }

        public double LastMonthProfitPercent { get; private set; }
    }
}