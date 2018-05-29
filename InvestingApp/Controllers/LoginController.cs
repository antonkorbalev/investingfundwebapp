using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InvestingApp.Models;

namespace InvestingApp.Controllers
{
    public class LoginController : Controller
    {
        [HttpPost]
        public ActionResult Index(LoginModel loginInfo)
        {
            //ModelState.AddModelError("Status", "Error!");

            return View(loginInfo);
        }

        public ActionResult Index()
        {
            ViewBag.Title = "Login";
            return View();
        }
    }
}