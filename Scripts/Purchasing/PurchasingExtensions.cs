using System.Collections.Generic;
using Currencies;
using Dirichlet.Numerics;
using UnityEngine.Purchasing;
using Price = Currencies.Price;

namespace Scripts.Purchasing
{
    public static class PurchasingExtensions
    {
        // Does not actually apply the payouts. Do that yourself.
        public static List<Price> GetPayouts(this Product product)
        {
            List<Price> retval = new List<Price>();
            foreach (var payout in product.definition.payouts)
            {
                if (payout.type == PayoutType.Other) continue;
                var currency = CurrencyManager.Instance.TryGet(payout.subtype);
                if (currency != null)
                {
                    var amount = (UInt128) payout.quantity;
                    retval.Add(new Price(currency, amount));
                }
            }
            return retval;
        }
    }
}