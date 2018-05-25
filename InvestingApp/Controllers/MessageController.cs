using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Collections.Specialized;
using InvestingApp.Models;
using System.Text;
using System.Web.Script.Serialization;
using System.Net.Mail;

namespace InvestingApp.Controllers
{
    public class MessageController : Controller
    {

        private bool checkCaptcha()
        {
            CaptchaValidationAnswer answer;
            using (var client = new WebClient())
            {
                var values = new NameValueCollection();
                values["secret"] = System.Web.Configuration.WebConfigurationManager.AppSettings["captcha_key"];
                values["response"] = Request.Form["g-recaptcha-response"];
                var response = Encoding.UTF8.GetString(client.UploadValues("https://www.google.com/recaptcha/api/siteverify", "POST", values));
                answer = new JavaScriptSerializer().Deserialize<CaptchaValidationAnswer>(response);
                return answer.Success;
            }
        }

        private bool checkMessage(Message message)
        {
            if (string.IsNullOrWhiteSpace(message.Name)
                || string.IsNullOrWhiteSpace(message.EMail)
                || string.IsNullOrWhiteSpace(message.Phone)
                || string.IsNullOrWhiteSpace(message.Content))
                return false;
            return true;
        }

        [HttpPost]
        public PartialViewResult SendMessage(Message message)
        {
            var success = true;
            if (!checkCaptcha() || (!checkMessage(message)))
                success = false;

            if (success)
            {
                var mailMessage = new MailMessage();
                mailMessage.From = new MailAddress("web@akinvest.tech");
                mailMessage.To.Add(System.Web.Configuration.WebConfigurationManager.AppSettings["email_to"]);
                mailMessage.Subject = "New message from AK investing techs";
                mailMessage.IsBodyHtml = true;
                var bld = new StringBuilder();
                bld.AppendFormat("<b>Name:</b> {0}", message.Name);
                bld.AppendLine("<br>");
                bld.AppendFormat("<b>Email:</b> {0}", message.EMail);
                bld.AppendLine("<br>");
                bld.AppendFormat("<b>Phone:</b> {0}", message.Phone);
                bld.AppendLine("<br>");
                bld.AppendLine(message.Content.Replace(Environment.NewLine, "<br>"));
                mailMessage.Body = bld.ToString();

                var client = new SmtpClient("localhost", 25);
                try
                {
                    client.Send(mailMessage);
                }
                catch
                {
                    success = false;
                }
            }

            var status = new RequestStatusMessage()
            {
                Result = success ? string.Format("Thank you, {0}. Your message has been sent.", message.Name) : "Error! Invalid data.",
                IsSuccess = success
            };
            return PartialView(status);
        }

        // GET: Message
        public ActionResult Index()
        {
            ViewBag.Title = "Send a message";
            return View(new Message());
        }
    }
}