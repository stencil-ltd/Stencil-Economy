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

        public bool CanAfford 
            => Currency.CanSpend(Amount);

        public CurrencyOperation Purchase()
            => Currency.Spend(Amount);

        public CurrencyOperation Receive()
            => Currency.Add(Amount);
        
        public static implicit operator Currency(Price price)
            => price.Currency;
    }
}