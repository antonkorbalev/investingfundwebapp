using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InvestingApp.Models.Margins;
using System.Net;
using System.Web.Configuration;
using System.Xml;
using System.Globalization;

namespace InvestingApp.Controllers
{
    public class MarginCalculatorController : Controller
    {
        // GET: MarginCalculator
        public ActionResult Index()
        {
            ViewBag.Title = "Margin calculator";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public PartialViewResult GetMargin(MarginCalculationRequest calcRequest)
        {
            var result = new CalculatedMargin();
            if (string.IsNullOrWhiteSpace(calcRequest.Instrument))
                return PartialView(result);

            try
            {
                if (calcRequest.StockExchange == StockExchange.MOEX)
                {
                    using (var w = new WebClient())
                    {
                        var dataType = string.Empty;
                        switch (calcRequest.Type)
                        {
                            case InstrumentType.Futures:
                                dataType = "F";
                                break;
                            case InstrumentType.Options:
                                dataType = "O";
                                break;
                        }
                        var xmlStr = w.DownloadString(string.Format(WebConfigurationManager.AppSettings["MOEX_EXCHANGE_URL"], dataType));

                        var xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(xmlStr);

                        var attrsDict = new Dictionary<string, string>();
                        foreach (XmlNode item in xmlDoc.SelectNodes("rtsdata/contract/item"))
                        {
                            attrsDict.Clear();
                            foreach (XmlAttribute attr in item.Attributes)
                                attrsDict.Add(attr.Name, attr.Value);
                            if (attrsDict.Values.Any(o => o.ToLower() == calcRequest.Instrument.ToLower()))
                            {
                                result.Money = calcRequest.Money;
                                result.InstrumentName = attrsDict["symbol"];
                                result.InstrumentCode = attrsDict["code"];
                                var dateStr = attrsDict["delivery_date"];
                                var y = int.Parse(dateStr.Substring(0, 4));
                                var m = int.Parse(dateStr.Substring(4, 2));
                                var d = int.Parse(dateStr.Substring(6, 2));
                                result.DeliveryDate = new DateTime(y, m, d);
                                result.InitialMargin = double.Parse(attrsDict["initial_margin"], CultureInfo.InvariantCulture);
                                result.InitialMarginPercent = double.Parse(attrsDict["initial_margin_percent"], CultureInfo.InvariantCulture);

                                result.Success = true;
                                return PartialView(result);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return PartialView(result);
        }
    }
}