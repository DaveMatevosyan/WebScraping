using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ListAmScraper
{
    class ListAmScraper : BaseScraper
    {
        public override string SimpleUrl => "https://www.list.am/am/category?q=";
        public override string FilterChanger => "&crc=-1&n=0&";
        public override string PageChanger => "&pg=";
        private readonly string DomainPart = "https://www.list.am";

        public override async Task<List<Item>> StartScraping(string searchText, int itemsCount = 1, int minPrice = 0, int maxPrice = 0)
        {
            int ItemsCount = 0;
            List<Item> SearchResults = new List<Item>();
            while (true)
            {
                string Url;
                if (minPrice != 0 || maxPrice != 0)
                {
                    Url = SimpleUrl.Insert(SimpleUrl.IndexOf('?') + 1, $"price1={minPrice}&price2={maxPrice}{FilterChanger}") +
                        searchText.Replace(' ', '+') + PageChanger + StartPageNum;
                }
                else
                {
                    Url = SimpleUrl + searchText.Replace(' ', '+') + PageChanger + StartPageNum;
                }

                HtmlDocument htmlDoc = await GetHtmlDocument(Url);
                List<HtmlNode> ProdsHtml;

                try
                {
                    ProdsHtml = htmlDoc.DocumentNode.Descendants("div")
                        .Where(node => node.GetAttributeValue("class", "")
                        .Equals("dl")).ToList();
                }
                catch (NullReferenceException)
                {
                    Console.WriteLine("Your search did not match any postings.");
                    return null;
                }

                var ProductsList = new List<HtmlNode>();

                foreach (HtmlNode Htmlnode in ProdsHtml)
                {
                    ProductsList.AddRange(Htmlnode.Descendants("a")
                    .Where(node => node.GetAttributeValue("href", "")
                    .Contains("item")).ToList());
                }

                Console.WriteLine($"LENGTH IS: {ProductsList.Count}");

                foreach (var item in ProductsList)
                {
                    if(ItemsCount >= itemsCount)
                    {
                        return SearchResults;
                    }

                    HtmlNode nameDiv = item.Descendants("div")
                        .FirstOrDefault().Descendants("div")
                        .FirstOrDefault();
                    string name = nameDiv == null ? "\n" : nameDiv.InnerText;
                    //Console.WriteLine($"Name: {name}");

                    string href = item.GetAttributeValue("href", "");
                    Console.WriteLine($"Href: {href}");
                    string realLink = DomainPart + href;

                    HtmlNode imgNode = item.Descendants("img")
                        .FirstOrDefault();
                    HtmlAttributeCollection attributes;
                    string imgLink;
                    if (imgNode != null)
                    {
                        attributes = imgNode.Attributes;

                        if (attributes.Contains("data-original"))
                            imgLink = imgNode.GetAttributeValue("data-original", "");

                        else if (attributes.Contains("src"))
                            imgLink = imgNode.GetAttributeValue("src", "");

                        else
                            imgLink = "\n";
                    }
                    else
                    {
                        imgLink = "\n";
                    }
                    Console.WriteLine($"ImgLink: {imgLink}");

                    HtmlNode priceNode = item.Descendants("div")
                        .Where(node => node.GetAttributeValue("class", "")
                        .Equals("p")).FirstOrDefault();
                    string price = priceNode == null ? "\n" : priceNode.InnerText;
                    Console.WriteLine($"Price: {price}");

                    HtmlNode categoryDiv = item?.Descendants("div")
                        .Where(node => node.GetAttributeValue("class", "")
                        .Equals("c")).FirstOrDefault();
                    string category = categoryDiv == null ? "\n" : categoryDiv.InnerText;
                    //Console.WriteLine($"Category: {category}");

                    HtmlNode dateDiv = item.Descendants("div")
                        .Where(node => node.GetAttributeValue("class", "")
                        .Equals("d")).FirstOrDefault();
                    string date = dateDiv == null ? "\n" : dateDiv.InnerText;
                    //Console.WriteLine($"Date: {date}");

                    Item result = new Item(realLink, imgLink, name, price);
                    SearchResults.Add(result);

                    Console.WriteLine();
                    ItemsCount++;
                }

                var PageSpan = htmlDoc.DocumentNode.Descendants("span")
                    .Where(node => node.GetAttributeValue("class", "")
                    .Equals("pp")).FirstOrDefault();
                if (PageSpan != null)
                {
                    HtmlNode PageHref;
                    PageHref = PageSpan.Descendants("a")
                    .Where(node => node.InnerText == (StartPageNum + 1).ToString()).FirstOrDefault();
                    if (PageHref != null)
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

