using System;
using Common;
using Currencies;

namespace Merch.Data
{
    [Serializable]
    public class MerchListing
    {
        public MerchItem Item;
        public Price MainPrice;
        public PriceArray ExtraPrices;
    }
}