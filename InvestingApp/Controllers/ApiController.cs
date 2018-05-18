using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InvestingApp.Database;
using InvestingApp.Database.Entities;

namespace InvestingApp.Controllers
{
    public class ApiController : Controller
    {
        public string Index()
        {
            return "Specify API command";
        }

        public string TakeBalance(string key, string date, string value)
        {
            if (!Request.IsLocal)
            {
                return "Not allowed.";
            }
            try
            {
                if (key != System.Configuration.ConfigurationManager.AppSettings["api_key"])
                    return "API key is invalid.";
                var d = DateTime.Parse(date);
                var v = double.Parse(value);

                using (var context = new InvestingContext())
                {
                    if (context.Balances.Any(o => o.DateTimeStamp == d))
                        return "Date value already exists.";
                    var row = new BalancesRow()
                    {
                        Balance = v,
                        DateTimeStamp = d
                    };
                    context.Balances.Add(row);
                    context.SaveChanges();
                }
            }
            catch 
            {
                return "Error";
            }
            return "OK";
        }
    }
}