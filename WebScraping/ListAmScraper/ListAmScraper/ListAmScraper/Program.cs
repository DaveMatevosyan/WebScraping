using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace ListAmScraper
{
    class Program
    {
        static void Main(string[] args)
        {
            //ListAmScraper tl = new ListAmScraper();
            //string search = "pix";
            //var results = tl.StartScraping(search).Result;

            //for (int i = 0; i < results.Count; i++)
            //{
            //    var result = results[i];
            //    FileStream f;
            //    using (f = File.Open($"{search}.txt", FileMode.Append))
            //    {
            //        using (StreamWriter s = new StreamWriter(f))
            //        {
            //            s.WriteLine($"ItemNumber: {i + 1}   {result}");
            //        }
            //    }
            //}

            //EbayScraper eb = new EbayScraper();
            //string search = "alcogel";
            //var results = eb.StartScraping(search, itemsCount: 139).Result;

            //for (int i = 0; i < results.Count; i++)
            //{
            //    var result = results[i];
            //    FileStream f;
            //    using (f = File.Open($"{search}.txt", FileMode.Append))
            //    {
            //        using (StreamWriter s = new StreamWriter(f))
            //        {
            //            s.WriteLine($"ItemNumber: {i + 1}   {result}");
            //        }
            //    }
            //}

            CurrencyScraper cs = new CurrencyScraper();
            var results = cs.StartScraping("USD", "AMD", 27).Result;
            Console.WriteLine(results);

            Console.ReadKey();
        }
    }
}
