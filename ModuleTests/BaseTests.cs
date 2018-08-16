using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using InvestingApp.Models;
using System.Collections.Generic;
using InvestingApp.Database.Entities;
using System.Linq;
using InvestingApp.Models.Rates;
using InvestingApp.Helpers;

namespace ModuleTests
{
    [TestClass]
    public class BaseTests
    {

        private GeneralInvestingInfo getInvestingInfo()
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

            var flows = new[] {
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
            };

            var ret = new GeneralInvestingInfo(data, flows);

            return ret;
        }

        [TestMethod]
        public void CheckGeneralProfit()
        {
            var data = getInvestingInfo();
            Assert.AreEqual(data.Profit, 15);
            Assert.AreEqual(data.ProfitPercent, 100);
            Assert.AreEqual(data.Max, 27);
            Assert.AreEqual(data.Min, -7);
            Assert.AreEqual(data.DrawDown, 9);    
        }

        [TestMethod]
        public void CheckDrawDown()
        {
            var data = new List<BalancesRow>();
            var flows = new FlowRow[] { };
            for (var i = 0; i < 1000; i++)
                data.Add(new BalancesRow() { Balance = i % 2 == 0 ? i : -i, DateTimeStamp = new DateTime(i) });
            var info = new GeneralInvestingInfo(data, flows);
            Assert.AreEqual(info.DrawDown, 1997);
        }

        [TestMethod]
        public void TestRatesForUSDollar()
        {
            var values = RatesDownloader.GetValues(RateType.USDollar,
                new DateTime(2018,1,1), new DateTime(2018, 3, 1));

            Assert.AreEqual(37, values.Count());
            Assert.AreEqual(56.4334, values.Last().Value);
        }

        [TestMethod]
        public void TestTicksRangeForAGraph()
        {
            var rangeStr = MVCStringsHelper.GetTicksRange(45000.45, 176000.86);
            Assert.AreEqual("[70000,90000,120000,140000,170000]", rangeStr);
            rangeStr = MVCStringsHelper.GetTicksRange(45000.45, 276000.86, divisor:100000, partsCount:2);
            Assert.AreEqual("[100000,200000]", rangeStr);
            rangeStr = MVCStringsHelper.GetTicksRange(45000.45, 176000.86, partsCount:2);
            Assert.AreEqual("[110000,170000]", rangeStr);
        }
    }
}
