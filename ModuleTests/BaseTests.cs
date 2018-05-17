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
            Assert.AreEqual(data.ProfitPercent, 300);
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
    }
}
