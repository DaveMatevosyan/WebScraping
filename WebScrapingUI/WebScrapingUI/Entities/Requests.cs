
using BaseScraperLibrary;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebScrapingUI.Entities
{
    public class Requests
    {
        [Key]
        public int RequestId { get; set; }

        public string Sites { get; set; }
        public string Currency { get; set; }
        public int? MinPrice { get; set; }
        public int? MaxPrice { get; set; }
        public string ItemName { get; set; }
        public int ItemCount { get; set; }
    }
}