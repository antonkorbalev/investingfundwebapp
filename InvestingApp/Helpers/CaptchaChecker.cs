using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using InvestingApp.Models;
using System.Collections.Specialized;
using System.Web.Script.Serialization;
using System.Net;
using System.Text;

namespace InvestingApp.Helpers
{
    internal static class CaptchaChecker
    {
        internal static bool CheckCaptcha(HttpRequestBase request)
        {
            CaptchaValidationAnswer answer;
            using (var client = new WebClient())
            {
                var values = new NameValueCollection();
                values["secret"] = System.Web.Configuration.WebConfigurationManager.AppSettings["captcha_key"];
                values["response"] = request.Form["g-recaptcha-response"];
                var response = Encoding.UTF8.GetString(client.UploadValues("https://www.google.com/recaptcha/api/siteverify", "POST", values));
                answer = new JavaScriptSerializer().Deserialize<CaptchaValidationAnswer>(response);
                return answer.Success;
            }
        }
    }
}