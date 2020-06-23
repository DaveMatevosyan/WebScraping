using BaseScraperLibrary;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Http;
using WebApi.Models;
using Newtonsoft.Json;
using System.IO;
using System.Web;
using System.Diagnostics;

namespace WebApi.Controllers
{
    public class ScraperController : ApiController
    {
        [HttpPost]
        [Route("api/Scraper/Search/")]
        public async Task<string> Get([FromBody] RequestModel model)
        {
            IEnumerable<Item> result = await StartSearch(model);

            return JsonConvert.SerializeObject(result, Formatting.Indented);
        }

        private async Task<List<Item>> StartSearch(RequestModel model)
        {
            List<string> sites = model.CheckedSites;
            string itemName = model.ItemName;
            int itemCount = model.ItemCount;
            int minPrice = model.MinPrice == null ? 0 : (int)model.MinPrice;
            int maxPrice = model.MaxPrice == null ? 0 : (int)model.MaxPrice;
            List<Item> AllItems = new List<Item>();

            //AutoResetEvent auto = new AutoResetEvent(false);
            List<Task> Tasks = new List<Task>();

            foreach (string s in sites)
            {
                Task task = Task.Run(async () =>
                {
                    Assembly asm = null;
                    try
                    {
                        string asmPath = HttpRuntime.AppDomainAppPath + $"Plugins/{s}Library.dll";
                        asm = Assembly.LoadFrom(asmPath);
                    }
                    catch (FileNotFoundException e)
                    {
                        Console.WriteLine(e.Message);
                        return;
                        //auto.Set();
                    }
                    catch (ArgumentException e)
                    {
                        Console.WriteLine(e.Message);
                        return;
                        //auto.Set();
                    }

                    var pluginTypes = from t in asm.GetTypes()
                                      where t.IsClass && (t.BaseType == typeof(BaseScraper))
                                      select t;

                    foreach (Type t in pluginTypes)
                    {
                        BaseScraper obj = (BaseScraper)Activator.CreateInstance(t);

                        int min = 0, max = 0;
                        if (minPrice != 0 || maxPrice != 0)
                        {
                            AvailableCurrencies currencyFrom = (AvailableCurrencies)model.Currency;
                            CurrencyAttribute curr_attr = t.GetCustomAttribute(typeof(CurrencyAttribute)) as CurrencyAttribute;
                            AvailableCurrencies currencyTo = curr_attr.Currency;
                            CurrencyScraper cs = new CurrencyScraper();

                            if (currencyFrom != currencyTo)
                            {
                                double rate = await cs.StartScraping(currencyFrom, currencyTo);
                                min = Convert.ToInt32(rate * minPrice);
                                max = Convert.ToInt32(rate * maxPrice);
                            }
                            else
                            {
                                min = minPrice;
                                max = maxPrice;
                            }
                        }

                        List<Item> items = await obj?.StartScraping(itemName, itemCount, min, max);
                        AllItems.AddRange(items);
                        //auto.Set();
                    }
                });
                Tasks.Add(task);
            }

            //foreach (Task t in Tasks)
            //    auto.WaitOne();

            await Task.WhenAll(Tasks);

            return AllItems;
        }

        [HttpGet]
        [Route("api/Scraper/GetSitesNames/")]
        public string GetSitesNames()
        {
            Stopwatch s = Stopwatch.StartNew();
            NameValueCollection nv = ConfigurationManager.AppSettings;
            string allscrapers = nv.Get("Plugins");
            string[] scrapers = allscrapers.Split(',');
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (string site in scrapers)
            {
                Assembly asm = null;
                try
                {
                    string asmPath = HttpRuntime.AppDomainAppPath + $"Plugins/{site}Library.dll";
                    asm = Assembly.LoadFrom(asmPath);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }

                var pluginTypes = from t in asm.GetTypes()
                                  where t.IsClass && (t.BaseType == typeof(BaseScraper))
                                  select t;

                foreach (Type t in pluginTypes)
                {
                    CategoryAttribute cat_attr = t.GetCustomAttribute(typeof(CategoryAttribute)) as CategoryAttribute;
                    AvailableCategories category = cat_attr != null ? cat_attr.Category : AvailableCategories.Other;
                    dict.Add(site, category.ToString());
                }
            }
            return JsonConvert.SerializeObject(dict);
        }
    }
}
