using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InvestingApp.Models;

namespace InvestingApp.Controllers
{
    public class MessageController : Controller
    {

        [HttpPost]
        public PartialViewResult SendMessage(Message message)
        {
            var result = string.Format("Thank you, {0}! Your message has been sent.", message.Name);
            var success = true;
            if (string.IsNullOrWhiteSpace(message.Name) 
                || string.IsNullOrWhiteSpace(message.EMail)
                || string.IsNullOrWhiteSpace(message.Phone)
                || string.IsNullOrWhiteSpace(message.Content))
            {
                result = "Invalid data. Please, fill all the fields! ";
                success = false;
            }
            
            var status = new RequestStatusMessage()
            {
                Result = result,
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