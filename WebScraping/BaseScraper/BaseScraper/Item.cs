using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseScraperLibrary
{
    public class Item
    {
        public string RealLink { get; }
        public string ImgLink { get; }
        public string Name { get; }
        public string Price { get; }
        public string SiteName { get; }

        public Item(string RealLink, string ImgLink, string Name, string Price, string SiteName)
        {
            this.RealLink = RealLink;
            this.ImgLink = ImgLink;
            this.Name = Name;
            this.Price = Price;
            this.SiteName = SiteName;
        }

        public override string ToString() => $"Name: {this.Name}\t Price: {this.Price}\t RealLink: {this.RealLink}" +
            $"\t ImgLink: {this.ImgLink}";
    }
}
