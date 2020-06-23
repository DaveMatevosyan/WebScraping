using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseScraperLibrary
{
    public enum AvailableCurrencies
    {
        AMD,
        RUB,
        USD,
        EUR
    } 

    [AttributeUsage(AttributeTargets.Class)]
    public class CurrencyAttribute : Attribute
    {
        public AvailableCurrencies Currency { get; }

        public CurrencyAttribute(AvailableCurrencies currency) => this.Currency = currency;
    }
}
