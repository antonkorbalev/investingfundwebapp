using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;
using System.Web.Mvc;
using System.Globalization;

namespace InvestingApp.Helpers
{
    public static class MVCStringsHelper
    {
        public static MvcHtmlString GetMVCDatesString(IEnumerable<DateTime> dates)
        {
            return new MvcHtmlString(string.Join(",", dates.Select(o => string.Format("'{0}'", o.ToString("MMM d yyyy", CultureInfo.InvariantCulture)))));
        }

        public static MvcHtmlString GetMVCDoublesString(IEnumerable<double> values)
        {
            return new MvcHtmlString(string.Join(",", values.Select(o => o.ToString("0.00", CultureInfo.InvariantCulture))));
        }

        public static MvcHtmlString GetMVCString(IEnumerable<string> strings)
        {
            return new MvcHtmlString(string.Join(",", strings.Select(o => string.Format("'{0}'", o))));
        }

        public static string GetCurrentCultureDesignation()
        {
            return CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
        }

        public static string GetTicksRange(double min, double max, int partsCount = 5, int divisor = 10000)
        {
            var ticksList = new List<double>();

            if (partsCount > 1)
            {
                var step = (max - min) / (partsCount);

                for (var i = 1; i <= partsCount; i++)
                {
                    var value = min + step * i;
                    ticksList.Add(value - value % divisor);
                }
            }

            return string.Format("[{0}]", string.Join(",", ticksList.Distinct().Select(o => Math.Floor(o))));
        }
    }
}