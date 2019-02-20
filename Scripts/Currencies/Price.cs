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
        public void SetAmount(UInt128 amount) => BigAmount = amount;

        public Price()
        {
        }

        public void Sync()
        {
            BigAmount = Amount;
        }

        public Price(Currency currency, UInt128 amount)
        {
            Currency = currency;
            BigAmount = amount;
        }
        
        public bool CanAfford 
            => Currency.CanSpend(BigAmount);

        public CurrencyOperation Apply(bool negative)
            => negative ? Purchase() : Receive();

        public CurrencyOperation Purchase()
            => Currency.Spend(BigAmount);

        public CurrencyOperation Receive()
            => Currency.Add(BigAmount);
        
        public static implicit operator Currency(Price price)
            => price.Currency;
    }
}