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
        
        public static implicit operator Currency(Price price)
            => price.Currency;
    }
}