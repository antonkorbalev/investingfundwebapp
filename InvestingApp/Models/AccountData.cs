using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InvestingApp.Database.Entities;

namespace InvestingApp.Models
{
    public class AccountData
    {
        public string UserName { get; }
        public double Benefit { get; }
        public double SharedRatio { get; }
        public IEnumerable<FlowRow> Flows { get; }
        public double CurrentBalance { get; }

        private double getPersonalValue(double value)
        {
            return Math.Round(value * (SharedRatio - Benefit) / 100, 2);
        }

        public AccountData(string name, double ratio, double currBalance, IEnumerable<FlowRow> flows)
        {
            UserName = name;
            SharedRatio = ratio;
            Flows = flows;
            CurrentBalance = currBalance;
            TotalProfitPercent = Math.Round(100 * TotalProfit / Flows.Sum(o => o.Payment), 2);
        }

        public double Money
        {
            get
            {
                return getPersonalValue(CurrentBalance);
            }
        }

        public double OthersMoney
        {
            get
            {
                return CurrentBalance - Money;
            }
        }

        public double TotalProfit
        {
            get
            {
                var flowsSum = Flows.Sum(o => o.Payment);
                return Money - flowsSum;
            }
        }

        public double TotalProfitPercent { get; }
    }
}