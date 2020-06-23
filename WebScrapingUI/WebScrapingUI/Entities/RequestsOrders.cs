using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebScrapingUI.Entities
{
    public class RequestsOrders
    {
        [Key]
        public int OrderId { get; set; }

        public string UserId { get; set; }
        public int RequestId { get; set; }
    }
}