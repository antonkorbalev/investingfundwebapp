using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.IO;
using System.Globalization;

namespace InvestingApp.Models.Rates
{
    public static class RatesDownloader
    {
        public static Dictionary<DateTime, double> GetValues(RateType rateType, DateTime dateFrom, DateTime dateTo)
        {
            var url = string.Empty;
            Dictionary<DateTime, double> res = new Dictionary<DateTime, double>();

            switch (rateType)
            {
                case RateType.USDollar:
                    url = string.Format("http://export.finam.ru/data.csv?market=41&em=82485&code=USDCB&apply=0&df={2}&mf={3}&yf={4}&from={0}&dt={5}&mt={6}&yt={7}&to={1}&p=8&f=USDCB_120101_181231&e=.csv&cn=USDCB&dtf=5&tmf=3&MSOR=1&mstime=on&mstimever=1&sep=3&sep2=2&datf=4",
                        dateFrom.ToShortDateString(), dateTo.ToShortDateString(),
                        dateFrom.Day, dateFrom.Month - 1, dateFrom.Year,
                        dateTo.Day, dateTo.Month - 1, dateTo.Year);
                    var client = new WebClient();
                    var ans = client.DownloadString(url);

                    using (var reader = new StringReader(ans))
                    {
                        string line;
                        double value;
                        DateTime dt;
                        while ((line = reader.ReadLine()) != null)
                        {
                            var parts = line.Split(';');
                            if (DateTime.TryParse(string.Format("{0} {1}", parts[2], parts[3]), CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                                if (double.TryParse(parts[4], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out value))
                                    res.Add(dt, value);
                        }
                    }

                    return res; 
            }

            return null;
        }
    }
}