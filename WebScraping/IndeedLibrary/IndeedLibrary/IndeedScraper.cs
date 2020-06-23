using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseScraperLibrary;

namespace IndeedLibrary
{
    [Currency(AvailableCurrencies.USD)]
    [Category(AvailableCategories.Vacancies)]
    public class IndeedScraper : BaseScraper
    {
        public override string SimpleUrl => "https://www.indeed.com/jobs?q=&limit=";
        public override string FilterChanger => throw new NotImplementedException();
        public override string PageChanger => "&start=";
        public override string SiteName => "indeed.com";
        private readonly string DomainPart = "https://www.indeed.com";
        private readonly int ItemsPerPage = 50;

        public override async Task<List<Item>> StartScraping(string searchText, int itemsCount = 0, int minPrice = 0, int maxPrice = 0)
        {
            int ItemsCount = 0;
            List<Item> SearchResults = new List<Item>();
            while (true)
            {
                string Url = SimpleUrl.Insert(SimpleUrl.IndexOf("&limit"), searchText.Replace(' ', '+')) + ItemsPerPage.ToString() + PageChanger + ((StartPageNum - 1)*ItemsPerPage).ToString();

                HtmlDocument htmlDoc = await GetHtmlDocument(Url);
                List<HtmlNode> ProdsHtml;

                try
                {
                    ProdsHtml = htmlDoc.DocumentNode.Descendants("td")
                        .Where(node => node.GetAttributeValue("id", "")
                        .Equals("resultsCol")).ToList();
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
                    .Contains("row result")).ToList());
                }

                Console.WriteLine($"LENGTH IS: {ProductsList.Count}");

                foreach (var item in ProductsList)
                {
                    if (ItemsCount >= (itemsCount == 2*ItemsPerPage ? itemsCount - 1 : itemsCount))
                    {
                        return SearchResults;
                    }
                    Console.WriteLine($"Item: {ItemsCount}");

                    HtmlNode nameTag = item.Descendants("h2")
                        .Where(node => node.GetAttributeValue("class", "")
                        .Equals("title")).FirstOrDefault();
                    HtmlNode companyTag = item.Descendants("div")
                        .Where(node => node.GetAttributeValue("class", "")
                        .Equals("sjcl")).FirstOrDefault();

                    string title = nameTag == null ? "\n" : nameTag.InnerText;
                    string company = companyTag == null ? "\n" : companyTag?.Descendants("span")
                        .Where(node => node.GetAttributeValue("class", "")
                        .Equals("company")).FirstOrDefault()?.InnerText;
                    string location = companyTag == null ? "\n" : companyTag?.Descendants("span")
                        .Where(node => node.GetAttributeValue("class", "")
                        .Contains("location")).FirstOrDefault()?.InnerText;
                    string Name = title + ", " + company + ", " + location;

                    HtmlNode hrefTag = item.Descendants("h2")
                        .Where(node => node.GetAttributeValue("class", "")
                        .Equals("title")).FirstOrDefault()?.Descendants("a")
                        .FirstOrDefault();
                    string realLink = hrefTag == null ? "\n" : DomainPart + hrefTag.GetAttributeValue("href", "");

                    string imgLink = "\n";

                    HtmlNode descTag = item.Descendants("div")
                        .Where(node => node.GetAttributeValue("class", "")
                        .Equals("summary")).FirstOrDefault();
                    HtmlNode priceTag = item.Descendants("span")
                        .Where(node => node.GetAttributeValue("class", "")
                        .Equals("salaryText")).FirstOrDefault();
                    HtmlNode dateTag = item.Descendants("span")
                        .Where(node => node.GetAttributeValue("class", "")
                        .Contains("date")).FirstOrDefault();
                    string date = dateTag == null ? "\n" : dateTag.InnerText;
                    string description = descTag == null ? "\n" : descTag.InnerText;
                    string price = priceTag == null ? "\n" : priceTag.InnerText;
                    string Description = description + "\n" + (priceTag != null ? "Salary - " + $"{price}." : " ") + "\n" + (dateTag != null ? "Posted - " + date : " ");

                    Item result = new Item(realLink, imgLink, Name, Description, SiteName);
                    SearchResults.Add(result);

                    Console.WriteLine();
                    ItemsCount++;
                }

                var PageSpan = htmlDoc.DocumentNode.Descendants("ul")
                    .Where(node => node.GetAttributeValue("class", "")
                    .Equals("pagination-list")).FirstOrDefault();
                if (PageSpan != null)
                {
                    HtmlNode PageHref;
                    PageHref = PageSpan.Descendants("a")
                    .Where(node => node.GetAttributeValue("aria-label", "") == (StartPageNum + 1).ToString()).FirstOrDefault();
                    if (PageHref != null)
                    {
                        StartPageNum++;
                    }
                    else
                    {
                        PageHref = PageSpan.Descendants("a")
                            .Where(node => node.GetAttributeValue("aria-label", "") == "Next").FirstOrDefault();
                        if (PageHref != null)
                            StartPageNum++;
                        else
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
