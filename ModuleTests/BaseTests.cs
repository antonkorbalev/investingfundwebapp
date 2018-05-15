using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using InvestingApp.Models;
using System.Collections.Generic;
using InvestingApp.Database.Entities;
using System.Linq;

namespace ModuleTests
{
    [TestClass]
    public class BaseTests
    {

        private GeneralInvestingInfo getInvestingInfo(bool withFlows)
        {
            var data = new[]
            {
                new BalancesRow()
                {
                    Balance = 5,
                    DateTimeStamp = new DateTime(2017, 12, 30)
                },
                new BalancesRow()
                {
                    Balance = 3,
                    DateTimeStamp = new DateTime(2018, 1, 5)
                },
                new BalancesRow()
                {
                    Balance = -5,
                    DateTimeStamp = new DateTime(2018, 1, 10)
                },
                new BalancesRow()
                {
                    Balance = -7,
                    DateTimeStamp = new DateTime(2018, 1, 20)
                },
                new BalancesRow()
                {
                    Balance = 9,
                    DateTimeStamp = new DateTime(2018, 2, 15)
                },
                new BalancesRow()
                {
                    Balance = 20,
                    DateTimeStamp = new DateTime(2018, 2, 27)
                },
                new BalancesRow()
                {
                    Balance = 27,
                    DateTimeStamp = new DateTime(2018, 3, 1)
                }
            };

            var flows = withFlows ? new[] {
                new FlowRow()
                {
                    DateTimeStamp = new DateTime(2017, 12, 30),
                    Payment = 5
                },
                new FlowRow()
                {
                    DateTimeStamp = new DateTime(2018, 1, 10),
                    Payment = -3
                },
                new FlowRow()
                {
                    DateTimeStamp = new DateTime(2018, 2, 15),
                    Payment = 10
                },
            } : null;

            var ret = new GeneralInvestingInfo(data, flows);

            return ret;
        }

        [TestMethod]
        public void CheckGeneralProfit()
        {
            var data = getInvestingInfo(false);
            Assert.AreEqual(data.Profit, 22);
            Assert.AreEqual(data.ProfitPercent, 440);
            Assert.AreEqual(data.Max, 27);
            Assert.AreEqual(data.Min, -7);
            Assert.AreEqual(data.DrawDown, 12);    
        }

        [TestMethod]
        public void CheckDrawDown()
        {
            var data = new List<BalancesRow>();
            for (var i = 0; i < 1000; i++)
                data.Add(new BalancesRow() { Balance = i % 2 == 0 ? i : -i });
            var info = new GeneralInvestingInfo(data, null);
            Assert.AreEqual(info.DrawDown, 1997);
        }

        [TestMethod]
        public void CheckProfitsWithFlows()
        {
            var data = getInvestingInfo(true);
            Assert.AreEqual(data.Profit, 10);
            Assert.AreEqual(data.ProfitPercent, 58.82);
        }

        [TestMethod]
        public void CheckProfitWithFlowsForLastMonth()
        {
            var data = getInvestingInfo(true);
            Assert.AreEqual(data.LastMonthProfit, 8);
            Assert.AreEqual(data.LastMonthProfitPercent, 52.63);
        }

        [TestMethod]
        public void CheckProfitsPerMonthDistr()
        {
            var data = getInvestingInfo(true);
            var profits = data.ProfitsPerMonth.Select(o => o.Value).ToArray();
            var percents = data.ProfitsPerMonth.Select(o => o.Percent).ToArray();
            Assert.IsTrue(profits.SequenceEqual(new double[] { -5, -7, 1, 0 }));
            Assert.IsTrue(percents.SequenceEqual(new double[] { -50, 0, 5.26, 0 }));
        }
    }
}
