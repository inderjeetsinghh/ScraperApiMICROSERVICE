using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using static MicroservicesApi.WebScraperModel;

namespace MicroservicesApi.Controllers
{
    [Route(template:"api/[controller]")]
    [ApiController]
    public class WebScraperController : ControllerBase
    {
        private readonly String websiteUrl = "https://www.techbitsolution.com";
        private readonly ILogger<WebScraperController> _logger;
        // Constructor
        public WebScraperController(ILogger<WebScraperController> logger)
        {
            _logger = logger;
        }


        private async Task<WebScraperModel> GetPageData(string url)
        {
            var config = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(url);

            WebScraperModel mymodel = new WebScraperModel();
           
            var HeadHtml = document.Head.InnerHtml;
            var AllHtml = document.DocumentElement.InnerHtml;
            var parser = new AngleSharp.Html.Parser.HtmlParser();

            var data = parser.ParseDocument(HeadHtml);
            var allHtmlData = parser.ParseDocument(AllHtml);
            var MetaTags = data.All.Where(x => x.LocalName == "meta");
            var AnchorTags = allHtmlData.QuerySelectorAll("a");
           
           

            var metaTitle = document.Title;
            var mataTagsKeywords = MetaTags.FirstOrDefault(x => x.GetAttribute("Name") == "keywords").GetAttribute("Content");
            var mataTagsDescription = MetaTags.FirstOrDefault(x => x.GetAttribute("Name") == "description").GetAttribute("Content");
            var AllLinks = AnchorTags.Where(x => x.GetAttribute("href").StartsWith("h")).ToList();
            var linksPath = AllLinks.Cast<IHtmlAnchorElement>()
                      .Select(m => m.Href)
                      .ToList();



            ChromeOptions options = new ChromeOptions();
            options.AddArgument("headless");//Comment if we want to see the window. 
            var driver = new ChromeDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), options);
            driver.Navigate().GoToUrl(url);
            var screenshot = (driver as ITakesScreenshot).GetScreenshot();
            screenshot.SaveAsFile(Guid.NewGuid() + ".png");
            driver.Close();
            driver.Quit();

            mymodel.title = metaTitle;
            mymodel.description = mataTagsDescription;
            mymodel.keywords = mataTagsKeywords;
            mymodel.AllHyperLinks = linksPath;
            mymodel.screenShot = screenshot;



            return mymodel;
        }

        private  Task<WebScraperModel> CheckForUpdates(string url)
        {
            
            return  GetPageData(url);

            // TODO: Diff the data
        }




        // GET: api/WebScraper
        [HttpGet(template:"getdata")]
        public Task<WebScraperModel> Get()
        {
         return CheckForUpdates(websiteUrl);
            //return new string[] { "value1", "value2" };
        }

        // GET: api/WebScraper/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/WebScraper
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/WebScraper/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
