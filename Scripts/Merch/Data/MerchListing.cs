using System;
using System.Collections.Generic;
using Common;
using Currencies;

namespace Merch.Data
{
    [Serializable]
    public class MerchListing
    {
        public MerchItem Item;
        public Price MainPrice;
        public List<Price> ExtraPrices;
        
        public static implicit operator MerchItem(MerchListing listing) 
            => listing.Item;
    }
}