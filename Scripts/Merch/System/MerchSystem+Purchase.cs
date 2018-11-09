using System.Collections.Generic;
using System.Linq;
using Currencies;
using JetBrains.Annotations;
using Merch.Data;

namespace Merch.System
{
    public partial class MerchSystem
    {
        [CanBeNull]
        private MerchListing GetListing(MerchItem item)
        {
            List<MerchListing> listings = null;
            _itemToListings.TryGetValue(item, out listings);
            return listings?[0];
        }

        [CanBeNull]
        private Price FindPrice(MerchItem item, Currency currency)
        {
            var listing = GetListing(item);
            if (listing == null) return null;
            var price = listing.MainPrice;
            if (price.Currency != currency)
                price = listing.ExtraPrices?.FirstOrDefault(price1 => price1.Currency == currency);
            return price;
        }
        
        public bool CanPurchase(MerchItem item, Currency currency)
        {
            var price = FindPrice(item, currency);
            return price != null && currency.CanSpend(price.Amount);
        }

        public bool AttemptPurchase(MerchItem item, Currency currency)
        {
            var price = FindPrice(item, currency);
            if (price == null) return false;
            var op = currency.Spend(price.Amount);
            if (!op.Success) return false;
            SetAcquired(item, true);
            op.AndSave();
            return true;
        }
    }
}