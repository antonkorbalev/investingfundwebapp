using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InvestingApp.Controllers
{
    public class PrivateController : Controller
    {
        // GET: Private
        public ActionResult Index()
        {
            if (!Request.IsAuthenticated)
                return RedirectToAction("Index", "Login");
            return View();
        }
    }
}