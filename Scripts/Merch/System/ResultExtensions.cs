using System.Collections.Generic;
using System.Linq;
using Currencies;

namespace Merch.System
{
    public static class ResultExtensions
    {
        public static bool QueryCurrencies(this MerchResult result, List<Currency> currencies)
        {
            if (currencies == null || currencies.Count == 0) return true;
            if (result.State.MainPrice != null && currencies.Contains(result.State.MainPrice.Currency))
                return true;
            if (result.State.ExtraPrices != null && currencies.Intersect(result.State.ExtraPrices.Select(price => price.Currency)).ToArray().Length > 0)
                return true;
            return false;
        }
    }
}