using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InvestingApp.Database;
using InvestingApp.Database.Entities;
using System.Net;

namespace InvestingApp.Controllers
{
    public class ApiController : Controller
    {
        public ActionResult Index()
        {
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        [HttpPost]
        public ActionResult TakeBalance(string key, string date, string value)
        {
            if (!Request.IsLocal)
            {
                return new HttpStatusCodeResult(HttpStatusCode.MethodNotAllowed);
            }
            try
            {
                if (key != System.Configuration.ConfigurationManager.AppSettings["api_key"])
                    return new HttpStatusCodeResult(HttpStatusCode.MethodNotAllowed);
                var d = DateTime.Parse(date);
                var v = double.Parse(value);

                using (var context = new InvestingContext())
                {
                    if (context.Balances.Any(o => o.DateTimeStamp == d))
                        new HttpStatusCodeResult(HttpStatusCode.NotAcceptable);
                    var row = new BalancesRow()
                    {
                        Balance = v,
                        DateTimeStamp = d
                    };
                    context.Balances.Add(row);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
    }
}