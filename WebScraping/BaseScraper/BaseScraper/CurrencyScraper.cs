using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BaseScraperLibrary
{
    public class CurrencyScraper
    {
        public string SimpleUrl => "https://exchangerate.guru////";

        public async Task<double> StartScraping(AvailableCurrencies from, AvailableCurrencies to, int amount = 1)
        {
            string Url = SimpleUrl.Insert(SimpleUrl.IndexOf("guru") + "guru".Length + 1, from.ToString());
            Url = Url.Insert(Url.IndexOf(from.ToString()) + from.ToString().Length + 1, to.ToString());
            Url = Url.Insert(Url.IndexOf(to.ToString()) + to.ToString().Length + 1, amount.ToString());

            HttpClient client = new HttpClient();
            string strHtml;
            try
            {
                strHtml = await client.GetStringAsync(Url);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc?.LoadHtml(strHtml);

            HtmlNode valueNode = htmlDoc.DocumentNode.Descendants("input")
                .Where(node => node.GetAttributeValue("data-role", "")
                .Equals("secondary-input")).FirstOrDefault();

            string exchange_rate = valueNode.GetAttributeValue("value", "");
            double rate = -1;
            try
            {
                if (!double.TryParse(exchange_rate, out rate))
                {
                    double.TryParse(exchange_rate.Replace('.', ','), out rate);
                }
            }
            catch (Exception)
            {
                return rate;
            }

            return rate;
        }
    }
}
