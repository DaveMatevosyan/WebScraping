using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using BaseScraperLibrary;

namespace WebApi.Models
{
    public class RequestModel
    {
        public Dictionary<string, bool> AllSites { get; set; }
        public List<string> CheckedSites { get; set; }

        public AvailableCurrencies? Currency { get; set; }
        public int? MinPrice { get; set; }
        public int? MaxPrice { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public string ItemName { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public int ItemCount { get; set; }

        public RequestModel()
        {
            CheckedSites = new List<string>();
            AllSites = new Dictionary<string, bool>();
        }
    }
}