using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InvestingApp.Database;
using InvestingApp.Database.Entities;
using System.Globalization;
using System.Threading.Tasks;
using InvestingApp.Helpers;

namespace InvestingApp.Models
{
    public class GeneralInvestingInfo
    {
        public const double RISK_FREE_RATE = 5;

        public Dictionary<DateTime, double> Profits { get; private set; }
        public IEnumerable<double> Savings { get; private set; }
        public IEnumerable<double> DollarBHs { get; private set; }
        public IEnumerable<double> MeanProfitsPerMonth { get; set; }

        public GeneralInvestingInfo(IEnumerable<BalancesRow> data, IEnumerable<FlowRow> flows, IEnumerable<Rate> ratesUSD = null)
        {
            Data = data;
            Flows = flows;
            if ((flows == null)
                || (Flows.Where(o => o.DateTimeStamp <= data.First().DateTimeStamp)
                .Sum(o => o.Payment) != data.First().Balance))
                    throw new InvalidOperationException("Error in database! Flows sum is not equal to initial balance.");
            Profits = StatsCalculator.GetProfits(data, flows);
            updateProfitsPerMonth();
            updateSavingsPerMonth();
            updateDollarBHs(ratesUSD);
        }

        private void updateProfitsPerMonth()
        {
            ProfitsPerMonth = StatsCalculator.GetProfitsByMonths(Profits, Data);

            double sum = 0;
            int n = 0;
            var means = new List<double>();
            foreach (var p in ProfitsPerMonth)
            {
                sum += p.Percent;
                n++;
                means.Add(Math.Round(sum / n, 2));
            }

            MeanProfitsPerMonth = means;
        }

        private void updateSavingsPerMonth()
        {
            Savings = StatsCalculator.GetSavingsPerMonth(Data, Flows, RISK_FREE_RATE);
        }

        private void updateDollarBHs(IEnumerable<Rate> ratesUSD)
        {
            DollarBHs = StatsCalculator.GetDollarBHs(ratesUSD, Profits, Flows);
        }

        private double calculateSharpeRatio()
        {
            return StatsCalculator.CalculateSharpeRatio(Data, Flows, RISK_FREE_RATE);
        }


        public double CurrentBalance
        {
            get
            {
                return Math.Round(Data.Last().Balance, 2);
            }
        }

        public double SharpeRatio
        {
            get
            {
                return calculateSharpeRatio();
            }
        }

        /// <summary>
        /// Profits for every month
        /// </summary>
        public IEnumerable<ProfitPerPeriod> ProfitsPerMonth { get; private set; }

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
                return Math.Round(100 * Profit / Flows.Sum(o => o.Payment) , 2);
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