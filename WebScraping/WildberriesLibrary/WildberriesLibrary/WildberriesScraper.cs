using BaseScraperLibrary;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WildberriesLibrary
{
    [Currency(AvailableCurrencies.RUB)]
    [Category(AvailableCategories.Various)]
    public class WildberriesScraper : BaseScraper
    {
        public override string SimpleUrl => "https://www.wildberries.ru/catalog/0/search.aspx?search=";
        public override string FilterChanger => "price=;&";
        public override string PageChanger => "&page=";
        public override string SiteName => "Wildberries.ru";

        public override async Task<List<Item>> StartScraping(string searchText, int itemsCount = 0, int minPrice = 0, int maxPrice = 0)
        {
            int ItemsCount = 0;
            List<Item> SearchResults = new List<Item>();
            while (true)
            {
                string Url;
                if (minPrice != 0 || maxPrice != 0)
                {
                    string pr1 = FilterChanger.Insert(FilterChanger.IndexOf(";"), minPrice.ToString());
                    string prices = pr1.Insert(pr1.IndexOf("&"), maxPrice.ToString());
                    Url = SimpleUrl + searchText.Replace(" ", "%20");
                    Url = Url.Insert(Url.IndexOf("search="), prices) + PageChanger + StartPageNum;
                }
                else
                {
                    Url = SimpleUrl + searchText.Replace(" ", "%20") + PageChanger + StartPageNum;
                }

                HtmlDocument htmlDoc = await GetHtmlDocument(Url);
                List<HtmlNode> ProdsHtml;

                try
                {
                    ProdsHtml = htmlDoc.DocumentNode.Descendants("div")
                        .Where(node => node.GetAttributeValue("class", "")
                        .Contains("catalog_main_table j-products-container")).ToList();
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
                    .Contains("j-card-item")).ToList());
                }

                Console.WriteLine($"LENGTH IS: {ProductsList.Count}");

                foreach (var item in ProductsList)
                {
                    Console.WriteLine($"Item: {ItemsCount}");
                    if (ItemsCount >= itemsCount)
                    {
                        return SearchResults;
                    }

                    HtmlNode nameTag = item.Descendants("span")
                        .Where(node => node.GetAttributeValue("class", "")
                        .Equals("goods-name")).FirstOrDefault();
                    string name = nameTag == null ? "\n" : nameTag.InnerText;

                    HtmlNode priceTag;
                    string price;
                    priceTag = item.Descendants("ins")
                        .Where(node => node.GetAttributeValue("class", "")
                        .Equals("lower-price")).FirstOrDefault();
                    if (priceTag != null)
                    {
                        price = priceTag.InnerText;
                    }
                    else
                    {
                        priceTag = item.Descendants("span")
                        .Where(node => node.GetAttributeValue("class", "")
                        .Equals("lower-price")).FirstOrDefault();

                        price = priceTag == null ? "\n" : priceTag.InnerText;
                    }

                    HtmlNode hrefTag = item.Descendants("a")
                        .Where(node => node.GetAttributeValue("class", "")
                        .Contains("ref_goods_n_p")).FirstOrDefault();
                    string realLink = hrefTag == null ? "\n" : hrefTag.GetAttributeValue("href", "");

                    HtmlNode imgTag = item.Descendants("img")
                        .Where(node => node.GetAttributeValue("class", "")
                        .Equals("thumbnail")).FirstOrDefault();
                    HtmlAttributeCollection attributes;
                    string imgLink;
                    if (imgTag != null)
                    {
                        attributes = imgTag.Attributes;

                        if (attributes.Contains("data-original"))
                            imgLink = imgTag.GetAttributeValue("data-original", "");

                        else if (attributes.Contains("src"))
                            imgLink = imgTag.GetAttributeValue("src", "");

                        else
                            imgLink = "\n";
                    }
                    else
                    {
                        imgLink = "\n";
                    }

                    Item result = new Item(realLink, imgLink, name, price, SiteName);
                    SearchResults.Add(result);

                    Console.WriteLine();
                    ItemsCount++;
                }

                var PageSpan = htmlDoc.DocumentNode.Descendants("div")
                    .Where(node => node.GetAttributeValue("class", "")
                    .Equals("pageToInsert")).FirstOrDefault();
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
