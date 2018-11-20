using System;
using UnityEngine;

namespace Currencies
{
    [Serializable]
    public class Price
    {
        [Tooltip("Null if IAP.")]
        public Currency Currency;
        public int Amount;

        public int GetAmount(bool multiply)
        {
            var retval = Amount;
            if (multiply) retval = (int) (retval * Currency.Multiplier());
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