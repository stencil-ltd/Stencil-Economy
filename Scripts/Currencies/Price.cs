using System;
using Dirichlet.Numerics;
using Scripts.Maths;
using UnityEngine;

namespace Currencies
{
    [Serializable]
    public class Price
    {
        [Tooltip("Null if IAP.")]
        public Currency Currency;
        
        [SerializeField]
        private ulong Amount;
        
        [SerializeField]
        [HideInInspector]
        private UInt128 BigAmount;

        public UInt128 GetAmount() => BigAmount.AtLeast(Amount);
        public void SetAmount(UInt128 amount)
        {
            BigAmount = amount;
            Amount = 0;
        }

        public Price()
        {
        }

        public Price(Currency currency, UInt128 amount)
        {
            Currency = currency;
            BigAmount = amount;
        }
        
        public bool CanAfford 
            => Currency.CanSpend(GetAmount());

        public CurrencyOperation Apply(bool negative)
            => negative ? Purchase() : Receive();

        public CurrencyOperation Purchase()
            => Currency.Spend(GetAmount());

        public CurrencyOperation Receive()
            => Currency.Add(GetAmount());
        
        public static implicit operator Currency(Price price)
            => price.Currency;
    }
}