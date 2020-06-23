using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ListAmScraper
{
    public abstract class BaseScraper
    {
        public abstract string SimpleUrl { get; }
        public abstract string FilterChanger { get; }
        public abstract string PageChanger { get; }
        public virtual int StartPageNum { get; protected set; } = 1;

        public abstract Task<List<Item>> StartScraping(string searchText, int itemsCount = 0, int minPrice = 0, int maxPrice = 0);
        
        public virtual async Task<HtmlDocument> GetHtmlDocument(string Url)
        {
            HttpClient client = new HttpClient();
            string strHtml;
            try
            {
                strHtml = await client.GetStringAsync(Url);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc?.LoadHtml(strHtml);

            return htmlDoc;
        }
    }
}
