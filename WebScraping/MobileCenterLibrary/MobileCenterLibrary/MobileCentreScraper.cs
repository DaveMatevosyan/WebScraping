using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseScraperLibrary;

namespace MobileCenterLibrary
{
    [Currency(AvailableCurrencies.AMD)]
    [Category(AvailableCategories.Phones_and_accessories)]
    public class MobileCentreScraper : BaseScraper
    {
        public override string SimpleUrl => "https://www.mobilecentre.am/search/?searchData=";
        public override string FilterChanger => throw new NotImplementedException();
        public override string PageChanger => throw new NotImplementedException();
        public override string SiteName => "mobilecentre.am";

        public override async Task<List<Item>> StartScraping(string searchText, int itemsCount = 0, int minPrice = 0, int maxPrice = 0)
        {
            int ItemsCount = 0;
            List<Item> SearchResults = new List<Item>();
            while (true)
            {
                string Url = SimpleUrl + searchText.Replace(' ', '+');

                HtmlDocument htmlDoc = await GetHtmlDocument(Url);
                List<HtmlNode> ProdsHtml;

                try
                {
                    ProdsHtml = htmlDoc.DocumentNode.Descendants("div")
                        .Where(node => node.GetAttributeValue("class", "")
                        .Contains("search-result")).ToList();
                }
                catch (NullReferenceException)
                {
                    Console.WriteLine("Your search did not match any postings.");
                    return null;
                }

                var ProductsList = new List<HtmlNode>();

                foreach (HtmlNode Htmlnode in ProdsHtml)
                {
                    ProductsList.AddRange(Htmlnode.Descendants("div")
                    .Where(node => node.GetAttributeValue("class", "")
                    .Equals("listitem")).ToList());
                }

                Console.WriteLine($"LENGTH IS: {ProductsList.Count}");

                foreach(var item in ProductsList)
                {
                    if (ItemsCount >= itemsCount)
                    {
                        return SearchResults;
                    }
                    Console.WriteLine($"Item: {ItemsCount + 1}");

                    HtmlNode nameTag = item.Descendants("div")
                        .Where(node => node.GetAttributeValue("class", "")
                        .Equals("item-body")).FirstOrDefault()?
                        .Descendants("h3").FirstOrDefault();
                    string name = nameTag == null ? "\n" : nameTag.InnerText;

                    HtmlNode hrefTag = item.Descendants("a")
                        .Where(node => node.GetAttributeValue("class", "")
                        .Equals("prod-item-img")).FirstOrDefault();
                    string realLink = hrefTag == null ? "\n" : hrefTag.GetAttributeValue("href", "");

                    HtmlNode imgTag = hrefTag?.Descendants("img").FirstOrDefault();
                    HtmlAttributeCollection attributes;
                    string imgLink;
                    if (imgTag != null)
                    {
                        attributes = imgTag.Attributes;

                        if (attributes.Contains("data-src"))
                            imgLink = imgTag.GetAttributeValue("data-src", "");

                        else if (attributes.Contains("src"))
                            imgLink = imgTag.GetAttributeValue("src", "");

                        else
                            imgLink = "\n";
                    }
                    else
                    {
                        imgLink = "\n";
                    }

                    HtmlNode priceNode = item.Descendants("div")
                        .Where(node => node.GetAttributeValue("class", "")
                        .Equals("item-body")).FirstOrDefault()?
                        .Descendants("div").Where(node => node.GetAttributeValue("class", "")
                        .Equals("price")).FirstOrDefault()?.Descendants("span").FirstOrDefault();
                    string price = priceNode == null ? "\n" : priceNode.InnerText;

                    Item result = new Item(realLink, imgLink, name, price, SiteName);
                    SearchResults.Add(result);

                    Console.WriteLine();
                    ItemsCount++;
                }

                return SearchResults;
            }
        }
    }
}
