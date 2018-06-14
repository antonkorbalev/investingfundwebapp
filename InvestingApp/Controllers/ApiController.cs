using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InvestingApp.Database;
using InvestingApp.Database.Entities;
using System.Net;
using InvestingApp.Models.Rates;

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

                SyncRates();
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        public void SyncRates()
        {
            using (var context = new InvestingContext())
            {
                var balancesDates = context.Balances.Select(o => o.DateTimeStamp).ToArray();
                var fromDate = context.Flows.Min(o => o.DateTimeStamp);
                var toDate = DateTime.Now;

                foreach (var type in Enum.GetValues(typeof(RateType)))
                {
                    var rateType = (RateType)type;
                    var data = RatesDownloader.GetValues(rateType, fromDate, toDate);

                    var existingRateDates = context.Rates.Where(o => o.Type == rateType)
                        .Select(o => o.DateTimeStamp);
                    var datesToAdd = balancesDates.Except(existingRateDates);

                    foreach (var d in datesToAdd.Where(o => data.Keys.Contains(o)))
                        context.Rates.Add(new Rate()
                        {
                            Type = rateType,
                            DateTimeStamp = d,
                            Value = data[d]
                        });
                    context.SaveChanges();
                }
            }
        }
    }
}