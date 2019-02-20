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
            MerchListing listing;
            _itemToListing.TryGetValue(item, out listing);
            return listing;
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
        
        public bool CanPurchase(MerchItem item, [CanBeNull] Currency currency)
        {
            if (currency == null) return false;
            var price = FindPrice(item, currency);
            return price != null && currency.CanSpend(price.GetAmount());
        }

        public bool AttemptPurchase(MerchItem item, Currency currency)
        {
            var price = FindPrice(item, currency);
            if (price == null) return false;
            var op = currency.Spend(price.GetAmount());
            if (!op.Success) return false;
            SetAcquired(item, true);
            op.AndSave();
            return true;
        }
    }
}