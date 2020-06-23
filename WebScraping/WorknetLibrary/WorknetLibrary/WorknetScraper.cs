using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseScraperLibrary;

namespace WorknetLibrary
{
    [Currency(AvailableCurrencies.AMD)]
    [Category(AvailableCategories.Vacancies)]
    public class WorknetScraper : BaseScraper
    {
        public override string SimpleUrl => "https://www.worknet.am/hy/jobs?query=";
        public override string FilterChanger => throw new NotImplementedException();
        public override string PageChanger => throw new NotImplementedException();
        public override string SiteName => "worknet.am";
        private readonly string DomainPart = "https://www.worknet.am";

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
                        .Contains("listview--bordered job-box")).ToList();
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
                    .Equals("card hover-shadow")).ToList());
                }

                Console.WriteLine($"LENGTH IS: {ProductsList.Count}");

                foreach (var item in ProductsList)
                {
                    if (ItemsCount >= itemsCount)
                    {
                        return SearchResults;
                    }
                    Console.WriteLine($"Item: {ItemsCount + 1}");

                    HtmlNode nameTag = item.Descendants("div")
                        .Where(node => node.GetAttributeValue("class", "")
                        .Equals("listview__content")).FirstOrDefault()?
                        .Descendants("div").Where(node => node.GetAttributeValue("class", "")
                        .Equals("listview__heading")).FirstOrDefault()?.Descendants("a").FirstOrDefault();
                    string name = nameTag == null ? "\n" : nameTag.InnerText;

                    string realLink = nameTag == null ? "\n" : DomainPart + nameTag.GetAttributeValue("href", "");

                    //HtmlNode imgTag = item.Descendants("div")
                    //    .Where(node => node.GetAttributeValue("class", "")
                    //    .Equals("listview__item")).FirstOrDefault()?.Descendants("img").FirstOrDefault();
                    //string imgLink = imgTag == null ? "\n" : DomainPart + imgTag.GetAttributeValue("src", "");
                    string imgLink = "\n";

                    HtmlNode priceNode = item.Descendants("div")
                        .Where(node => node.GetAttributeValue("class", "")
                        .Contains("listview__attrs")).FirstOrDefault()?
                        .Descendants("i")
                        .Where(node => node.GetAttributeValue("class", "")
                        .Equals("fa fa-money")).FirstOrDefault()?.ParentNode;
                    HtmlNode descNode = item.Descendants("div")
                        .Where(node => node.GetAttributeValue("class", "")
                        .Equals("listview__content")).FirstOrDefault()?
                        .Descendants("p").FirstOrDefault();
                    string description = descNode == null ? "\n" : descNode.InnerText;
                    string price = priceNode == null ? "\n" : priceNode.InnerText;
                    string Description = description + ":" + (priceNode != null ? "Աշխատավարձ - " + price : " ");

                    Item result = new Item(realLink, imgLink, name, Description, SiteName);
                    SearchResults.Add(result);

                    Console.WriteLine();
                    ItemsCount++;
                }

                return SearchResults;
            }
        }
    }
}
