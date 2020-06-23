using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ListAmScraper
{
    public class CurrencyScraper
    {
        public string SimpleUrl => "https://freecurrencyrates.com/en/#;;;;fcr";

        public async Task<int> StartScraping(string from, string to, int amount)
        {
            string Url = SimpleUrl.Insert(SimpleUrl.IndexOf("#") + 1, from);
            Url = Url.Insert(Url.IndexOf(";") + 1, to);
            Url = Url.Insert(Url.IndexOf(to) + to.Length + 1, amount.ToString());

            //Process.Start("chrome.exe", Url);
            HttpClient client = new HttpClient();
            string strHtml;
            try
            {
                strHtml = await client.GetStringAsync(Url);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc?.LoadHtml(strHtml);

            HtmlNode value = htmlDoc.DocumentNode.Descendants("input")
                .Where(node => node.GetAttributeValue("id", "")
                .Equals("value_to")).FirstOrDefault();

            string exchange_rate = value.GetAttributeValue("value", "");
            int rate = int.Parse(exchange_rate.Split('.')[0]);

            return rate * amount;
        }
    }
}
