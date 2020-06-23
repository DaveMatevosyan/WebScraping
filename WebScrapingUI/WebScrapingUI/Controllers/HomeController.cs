using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using WebApi.Models;
using Newtonsoft.Json.Linq;
using System.Web.Routing;
using BaseScraperLibrary;

namespace WebScrapingUI.Controllers
{
    public class HomeController : Controller
    {
        public HttpClient client = new HttpClient();

        public async Task<ActionResult> Index()
        {
            Dictionary<string, string> sites = await GetAllNames();
            ViewData["Sites"] = sites;
            return View();
        }

        private async Task<Dictionary<string, string>> GetAllNames()
        {
            Uri server_url = new Uri("https://localhost:44342/");
            client.BaseAddress = server_url;
            var response = await client.GetAsync("api/Scraper/GetSitesNames/");
            var contenteWithSlashes = await response.Content.ReadAsStringAsync();
            var strWithoutSlashes = JToken.Parse(contenteWithSlashes).ToString();
            Dictionary<string, string> sites = JsonConvert.DeserializeObject<Dictionary<string, string>>(strWithoutSlashes);
            return sites;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(RequestModel model)
        {
            Dictionary<string, string> sites = await GetAllNames();
            ViewData["Sites"] = sites;
            if (ModelState.IsValid)
            {
                foreach (string s in model.AllSites.Keys)
                {
                    if (model.AllSites[s] == true)
                        model.CheckedSites.Add(s);
                }
                if (model.CheckedSites.Count == 0)
                {
                    ModelState.AddModelError("", "at least one page must be selected");
                    //return JavaScript("location.reload(true)");
                    return View();
                }
                var cats_except_various = from s in model.CheckedSites
                                          where (AvailableCategories)Enum.Parse(typeof(AvailableCategories), sites[s]) != AvailableCategories.Various
                                          select (AvailableCategories)Enum.Parse(typeof(AvailableCategories), sites[s]);
                if (cats_except_various.Count() != 0)
                {
                    var other_first = cats_except_various.First();
                    var diff_cats = from c in cats_except_various
                                    where c != other_first
                                    select c;
                    if (diff_cats.Count() != 0)
                    {
                        ModelState.AddModelError("", "You can choose only one category of pages");
                        //return JavaScript("location.reload(true)");
                        return View();
                    }
                }
                if (model.MinPrice != null || model.MaxPrice != null)
                {
                    if (model.Currency == null)
                    {
                        ModelState.AddModelError("", "Currency must be selected");
                        //return JavaScript("location.reload(true)");
                        return View();
                    }
                    if (model.MinPrice >= model.MaxPrice)
                    {
                        ModelState.AddModelError("", "minimum price must be less than maximum price");
                        //return JavaScript("location.reload(true)");
                        return View();
                    }
                }
                TempData["model"] = model;
                //return JavaScript("window.location = '../Result/Result'");
                return RedirectToAction("Result", "Result");
            }
            //return JavaScript("location.reload(true)");
            return View();
        }
    }
}