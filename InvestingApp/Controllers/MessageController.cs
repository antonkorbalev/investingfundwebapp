using System;
using System.Web.Mvc;
using InvestingApp.Models;
using System.Text;
using System.Net.Mail;
using InvestingApp.Helpers;
using System.Web.Configuration;

namespace InvestingApp.Controllers
{
    public class MessageController : Controller
    {

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
        [ValidateAntiForgeryToken]
        public PartialViewResult SendMessage(Message message)
        {
            var success = true;
            if (!(CaptchaChecker.CheckCaptcha(Request) || (!checkMessage(message))))
                success = false;

            if (success)
            {
                var mailMessage = new MailMessage();
                mailMessage.From = new MailAddress("web@akinvest.tech");
                mailMessage.To.Add(WebConfigurationManager.AppSettings["email_to"]);
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

        [AllowAnonymous]
        public ActionResult Index()
        {
            ViewBag.Title = "Send a message";
            return View(new Message());
        }
    }
}