using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InvestingApp.Models;
using System.Security.Cryptography;
using System.Text;
using InvestingApp.Database;

namespace InvestingApp.Controllers
{
    public class LoginController : Controller
    {
        private const int MAX_ATTEMPTS = 3;
        private readonly TimeSpan attempts_interval = TimeSpan.FromMinutes(5);

        private string sha256(string randomString)
        {
            var crypt = new SHA256Managed();
            var hash = new StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.ASCII.GetBytes(randomString));
            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }
            return hash.ToString();
        }

        [HttpPost]
        public ActionResult Index(LoginModel loginInfo)
        {
            ViewBag.Title = "Login";

            using (var context = new InvestingContext())
            {
                var user = context.Users.SingleOrDefault(o => o.Login == loginInfo.Login);
                if (user == null)
                    ModelState.AddModelError("Status", string.Format("User with login {0} not found.", loginInfo.Login));
                else
                {
                    if ((DateTime.Now - user.LastAttemptTime) > attempts_interval)
                        user.LoginAttempts = 0;
                    user.LastAttemptTime = DateTime.Now;
                    user.LoginAttempts++;
                    if (user.LoginAttempts >= MAX_ATTEMPTS)
                        ModelState.AddModelError("Status", string.Format("Too many attempts for user with login {0}. Please, wait.", loginInfo.Login));
                    
                    var hash = sha256(loginInfo.Password ?? string.Empty);
                    if (hash == user.Password)
                    {
                        
                    }
                    else
                        ModelState.AddModelError("Status", "Invalid password.");
                }
            }
           
            return View(loginInfo);
        }

        public ActionResult Index()
        {
            ViewBag.Title = "Login";
            return View();
        }
    }
}