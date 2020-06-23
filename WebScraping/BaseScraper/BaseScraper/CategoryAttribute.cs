using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseScraperLibrary
{
    public enum AvailableCategories
    {
        Various,
        Phones_and_accessories,
        Appliances,
        Vacancies,
        Other
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class CategoryAttribute : Attribute
    {
        public AvailableCategories Category { get; }

        public CategoryAttribute(AvailableCategories category) => this.Category = category;
    }
}
