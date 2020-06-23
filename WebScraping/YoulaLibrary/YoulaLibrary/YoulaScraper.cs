using BaseScraperLibrary;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoulaLibrary
{
    [Currency(AvailableCurrencies.RUB)]
    [Category(AvailableCategories.Various)]
    public class YoulaScraper : BaseScraper
    {
        public override string SimpleUrl => "https://youla.ru/?q=";
        public override string FilterChanger => "attributes[price][from]=00&attributes[price][to]=00&";
        public override string PageChanger => "&page=";
        public override string SiteName => "Youla.ru";
        private readonly string DomainPart = "https://youla.ru";

        public override async Task<List<Item>> StartScraping(string searchText, int itemsCount = 0, int minPrice = 0, int maxPrice = 0)
        {
            int ItemsCount = 0;
            List<Item> SearchResults = new List<Item>();
            while (true)
            {
                string Url;
                if (minPrice != 0 || maxPrice != 0)
                {
                    Url = SimpleUrl + searchText.Replace(" ", "%20");
                    Url = Url.Insert(Url.IndexOf("?") + 1, FilterChanger.Insert(FilterChanger.IndexOf("[from]=") + "[from]=".Length, minPrice.ToString()));
                    Url = Url.Insert(Url.IndexOf("[to]=") + "[to]=".Length, maxPrice.ToString()) + PageChanger + StartPageNum;
                }
                else
                {
                    Url = SimpleUrl + searchText.Replace(" ", "%20") + PageChanger + StartPageNum;
                }
                Console.WriteLine($"PageNumber: {StartPageNum}");

                HtmlDocument htmlDoc = await GetHtmlDocument(Url);
                List<HtmlNode> ProdsHtml;

                try
                {
                    ProdsHtml = htmlDoc.DocumentNode.Descendants("ul")
                        .Where(node => node.GetAttributeValue("class", "")
                        .Contains("product_list")).ToList();
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
                    .Where(node => node.GetAttributeValue("class", "")
                    .Contains("product_item")).ToList());
                }

                Console.WriteLine($"LENGTH IS: {ProductsList.Count}");

                foreach (var item in ProductsList)
                {
                    Console.WriteLine($"Item: {ItemsCount}");
                    if (ItemsCount >= itemsCount)
                    {
                        return SearchResults;
                    }

                    HtmlNode nameTag = item.Descendants("div")
                        .Where(node => node.GetAttributeValue("class", "")
                        .Equals("product_item__title")).FirstOrDefault();
                    string name = nameTag == null ? "\n" : nameTag.InnerText;

                    HtmlNode priceTag = item.Descendants("div")
                        .Where(node => node.GetAttributeValue("class", "")
                        .Equals("product_item__description ")).FirstOrDefault()?
                        .Descendants("div").FirstOrDefault();
                    string price = priceTag == null ? "\n" : priceTag.InnerText.Split('&')[0];

                    HtmlNode hrefTag = item.Descendants("a").FirstOrDefault();
                    string realLink = hrefTag == null ? "\n" : DomainPart + hrefTag.GetAttributeValue("href", "");

                    HtmlNode imgTag = item.Descendants("div")
                        .Where(node => node.GetAttributeValue("class", "")
                        .Equals("product_item__image")).FirstOrDefault()?
                        .Descendants("image").FirstOrDefault();
                    string imgLink = imgTag == null ? "\n" : imgTag.GetAttributeValue("xlink:href", "");

                    Item result = new Item(realLink, imgLink, name, price, SiteName);
                    SearchResults.Add(result);

                    ItemsCount++;

                }

                var PageSpan = htmlDoc.DocumentNode.Descendants("a")
                    .Where(node => node.GetAttributeValue("class", "")
                    .Contains("_paginator_next_button")).FirstOrDefault();

                if (PageSpan != null)
                {
                    StartPageNum++;
                }
                else
                {
                    return SearchResults;
                }
            }
        }
    }
}
