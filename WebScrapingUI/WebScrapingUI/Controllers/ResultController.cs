using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.IO;
using System.Web.Mvc;
using WebApi.Models;
using System.Collections;
using System.Text;
using System.Diagnostics;
using BaseScraperLibrary;

namespace WebScrapingUI.Controllers
{
    public class ResultController : Controller
    {
        private HttpClient client = new HttpClient();

        // GET: Result
        public async Task<ActionResult> Result()
        {
            string serialized = TempData["serialized_Cookie"] as string;
            HttpCookie res_cookie = HttpContext.Request.Cookies["Results"];
            if (res_cookie == null)
            {
                HttpCookie cookie = new HttpCookie("Results");
                cookie.Value = HttpUtility.UrlEncode(serialized);
                HttpContext.Response.Cookies.Add(cookie);
            }
            else
            {
                if (serialized != null)
                    res_cookie.Value = HttpUtility.UrlEncode(serialized);
            }

            RequestModel model = TempData["model"] as RequestModel;
            IEnumerable<Item> Results = await GetItems(model);
            ViewData["Count"] = Results?.Count();
            return View(Results);
        }

        [HttpPost]
        private async Task<IEnumerable<Item>> GetItems(RequestModel model)
        {
            Uri server_url = new Uri("https://localhost:44342/");
            client.BaseAddress = server_url;

            if (model == null)
            {
                HttpCookie resultsCookie = HttpContext.Request.Cookies["Results"];
                if (resultsCookie != null)
                {
                    string serialized_data = HttpUtility.UrlDecode(resultsCookie.Value);
                    TempData["serialized_Cookie"] = serialized_data;
                    if (serialized_data != null)
                    {
                        List<Item> resultWithCookies = JsonConvert.DeserializeObject<List<Item>>(serialized_data);
                        return resultWithCookies;
                    }
                    return null;
                }
            }

            string content = JsonConvert.SerializeObject(model);
            Stopwatch s = Stopwatch.StartNew();
            var response = await client.PostAsync("api/Scraper/Search/", new StringContent(content, Encoding.UTF8, "application/json"));
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            string contenteWithSlashes = await response.Content.ReadAsStringAsync();
            string strWithoutSlashes = JToken.Parse(contenteWithSlashes).ToString();

            TempData["serialized_Cookie"] = strWithoutSlashes;

            List<Item> result = JsonConvert.DeserializeObject<List<Item>>(strWithoutSlashes);

            double timeElapsed = s.ElapsedMilliseconds;
            ViewData["time"] = timeElapsed;
            HttpContext.Session["model"] = model;
            return result;
        }
    }
}