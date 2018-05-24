using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InvestingApp.Models
{
    public class CaptchaValidationAnswer
    {
        public bool Success { get; set; }
        public DateTime Challenge_ts { get; set; }
        public string HostName { get; set; }
    }
}