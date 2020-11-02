using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp;

namespace MicroservicesApi
{
    public class WebScraperModel
    {
        public WebScraperModel()
        {
        }
        
        public string title { get; set; }
        public string description { get; set; }
        public string keywords { get; set; }
        public List<string> AllHyperLinks { get; set; }
        public OpenQA.Selenium.Screenshot screenShot { get; set; }
    }


}
