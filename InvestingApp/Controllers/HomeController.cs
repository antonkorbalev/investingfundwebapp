using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InvestingApp.Models;
using InvestingApp.Database;
using InvestingApp.Models.Rates;

namespace InvestingApp.Controllers
{
    public class HomeController : Controller
    {
        private GeneralInvestingInfo getGeneralInfo()
        {
            using (var context = new InvestingContext())
            {
                var data = context.Balances.OrderBy(o => o.Id).ToArray();
                var flows = context.Flows.OrderBy(o => o.Id).ToArray();
                var ratesUSD = context.Rates.Where(o => o.Type == RateType.USDollar).OrderBy(o => o.Id).ToArray();
                return new GeneralInvestingInfo(data, flows, ratesUSD);       
            }
        }

        // GET: Home
        public ActionResult Index()
        {
            ViewBag.Title = "Investing";
            return View(getGeneralInfo());
        }
    }
}