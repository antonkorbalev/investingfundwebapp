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
    }
}