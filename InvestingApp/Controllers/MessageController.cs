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
        public ActionResult Index(MessageModel message)
        {
            
            return View();
        }

        // GET: Message
        public ActionResult Index()
        {
            ViewBag.Title = "Send a message";
            return View(new MessageModel());
        }
    }
}