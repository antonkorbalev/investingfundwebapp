using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InvestingApp.Models;
using InvestingApp.Database;

namespace InvestingApp.Controllers
{
    public class HomeController : Controller
    {
        private GeneralInvestingInfo getGeneralInfo()
        {
            using (var context = new InvestingContext())
            {
                var data = context.Balances.ToArray();
                var flows = context.Flows.ToArray();
                return new GeneralInvestingInfo(data, flows);       
            }
        }

        // GET: Home
        public ActionResult Index()
        {
            ViewBag.Title = "About";
            return View(getGeneralInfo());
        }
    }
}