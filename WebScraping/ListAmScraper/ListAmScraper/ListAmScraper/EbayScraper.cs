using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListAmScraper
{
    public class EbayScraper : BaseScraper
    {
        public override string SimpleUrl => "https://www.ebay.com/sch/i.html?_from=R40&_nkw=&_sacat=0";
        public override string FilterChanger => "&rt=nc&_udlo=&_udhi=";
        public override string PageChanger => "&_pgn=";

        public override async Task<List<Item>> StartScraping(string searchText, int itemsCount = 1, int minPrice = 0, int maxPrice = 0)
        {
            int ItemsCount = 0;
            List<Item> SearchResults = new List<Item>();
            while (true)
            {
                string Url;
                if(minPrice !=0 || maxPrice != 0)
                {
                    Url = SimpleUrl.Insert(SimpleUrl.IndexOf("&_sacat"), searchText.Replace(' ', '+'));
                    Url += FilterChanger.Insert(FilterChanger.IndexOf("&_udhi"), minPrice.ToString()) + maxPrice.ToString()
                        + PageChanger + StartPageNum;
                }
                else
                {
                    Url = SimpleUrl.Insert(SimpleUrl.IndexOf("&_sacat"), searchText.Replace(' ', '+')) + PageChanger + StartPageNum;
                    Console.WriteLine($"PageNumber: {StartPageNum}");
                }

                HtmlDocument htmlDoc = await GetHtmlDocument(Url);
                List<HtmlNode> ProdsHtml;

                try
                {
                    ProdsHtml = htmlDoc.DocumentNode.Descendants("ul")
                        .Where(node => node.GetAttributeValue("class", "")
                        .Contains("srp-results")).ToList();
                }
                catch (NullReferenceException)
                {
                    Console.WriteLine("Your search did not match any postings.");
                    return null;
                }

                var ProductsList = new List<HtmlNode>();

                foreach (HtmlNode Htmlnode in ProdsHtml)
                {
                    ProductsList.AddRange(Htmlnode.Descendants("li")
                    .Where(node => node.GetAttributeValue("id", "")
                    .Contains("listing")).ToList());
                }

                Console.WriteLine($"LENGTH IS: {ProductsList.Count}");

                foreach (var item in ProductsList)
                {
                    Console.WriteLine($"Item: {ItemsCount}");
                    if(ItemsCount >= itemsCount)
                    {
                        return SearchResults;
                    }

                    HtmlNode nameTag = item.Descendants("div")
                        .Where(node => node.GetAttributeValue("class", "")
                        .Equals("s-item__info clearfix")).FirstOrDefault()
                        .Descendants("a").FirstOrDefault().Descendants("h3")
                        .FirstOrDefault();
                    string name = nameTag == null ? "\n" : nameTag.InnerText;

                    HtmlNode priceTag = item.Descendants("div")
                        .Where(node => node.GetAttributeValue("class", "")
                        .Equals("s-item__info clearfix")).FirstOrDefault()
                        .Descendants("div").Where(node => node.GetAttributeValue("class", "")
                        .Equals("s-item__details clearfix")).FirstOrDefault()
                        .Descendants("span").Where(node => node.GetAttributeValue("class", "")
                        .Equals("s-item__price")).FirstOrDefault();
                    string price = priceTag == null ? "\n" : priceTag.InnerText;

                    HtmlNode hrefTag = item.Descendants("a")
                        .Where(node => node.GetAttributeValue("class", "")
                        .Equals("s-item__link")).FirstOrDefault();
                    string realLink = hrefTag == null ? "\n" : hrefTag.GetAttributeValue("href", "");

                    HtmlNode imgTag = item.Descendants("img")
                        .Where(node => node.GetAttributeValue("class", "")
                        .Equals("s-item__image-img")).FirstOrDefault();
                    string imgLink = imgTag == null ? "\n" : imgTag.GetAttributeValue("src", "");

                    Item result = new Item(realLink, imgLink, name, price);
                    SearchResults.Add(result);

                    Console.WriteLine();
                    ItemsCount++;
                }

                var PageSpan = htmlDoc.DocumentNode.Descendants("ol")
                    .Where(node => node.GetAttributeValue("class", "")
                    .Equals("x-pagination__ol")).FirstOrDefault();
                if (PageSpan != null)
                {
                    HtmlNode PageHref;
                    PageHref = PageSpan.Descendants("li")
                        .Where(node => node.InnerText == (StartPageNum + 1).ToString()).FirstOrDefault();
                    if(PageHref != null)
                    {
                        StartPageNum++;
                    }
                    else
                    {
                        return SearchResults;
                    }
                }
                else
                {
                    return SearchResults;
                }
            }
        }
    }
}
