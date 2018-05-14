using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using InvestingApp.Models;
using System.Collections.Generic;
using InvestingApp.Database.Entities;

namespace ModuleTests
{
    [TestClass]
    public class BaseTests
    {

        private GeneralInvestingInfo getInvestingInfo()
        {
            var data = new GeneralInvestingInfo();
            data.Data = new[]
            {
                new BalancesRow()
                {
                    Balance = 10,
                    DateTimeStamp = new DateTime(1)
                },
                new BalancesRow()
                {
                    Balance = 3,
                    DateTimeStamp = new DateTime(2)
                },
                new BalancesRow()
                {
                    Balance = -5,
                    DateTimeStamp = new DateTime(3)
                },
                new BalancesRow()
                {
                    Balance = -7,
                    DateTimeStamp = new DateTime(4)
                },
                new BalancesRow()
                {
                    Balance = 9,
                    DateTimeStamp = new DateTime(5)
                },
                new BalancesRow()
                {
                    Balance = 20,
                    DateTimeStamp = new DateTime(4)
                },
                new BalancesRow()
                {
                    Balance = 17,
                    DateTimeStamp = new DateTime(5)
                }
            };

            return data;
        }

        [TestMethod]
        public void CheckGeneralProfit()
        {
            var data = getInvestingInfo();
            Assert.AreEqual(data.Profit, 7);
            Assert.AreEqual(data.ProfitPercent, 70);
            Assert.AreEqual(data.Max, 20);
            Assert.AreEqual(data.Min, -7);
            Assert.AreEqual(data.DrawDown, 17);    
        }

        [TestMethod]
        public void CheckDrawDown()
        {
            var data = new List<BalancesRow>();
            for (var i = 0; i < 1000; i++)
                data.Add(new BalancesRow() { Balance = i % 2 == 0 ? i : -i });
            var info = new GeneralInvestingInfo() { Data = data };
            Assert.AreEqual(info.DrawDown, 1997);
        }
    }
}
