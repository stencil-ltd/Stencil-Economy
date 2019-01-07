using System;
using UnityEngine;

namespace Currencies
{
    [Serializable]
    public class Price
    {
        [Tooltip("Null if IAP.")]
        public Currency Currency;
        public ulong Amount;

        public Price()
        {
        }

        public Price(Currency currency, ulong amount)
        {
            Currency = currency;
            Amount = amount;
        }

        public ulong GetAmount(bool multiply)
        {
            var retval = Amount;
            if (multiply) retval = (ulong) (retval * Currency.Multiplier());
            return retval;
        }
        
        public bool CanAfford 
            => Currency.CanSpend(Amount);

        public CurrencyOperation Apply(bool negative)
            => negative ? Purchase() : Receive();

        public CurrencyOperation Purchase()
            => Currency.Spend(Amount);

        public CurrencyOperation Receive()
            => Currency.Add(Amount);
        
        public static implicit operator Currency(Price price)
            => price.Currency;
    }
}