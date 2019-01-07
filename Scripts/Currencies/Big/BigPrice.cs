using System;
using Dirichlet.Numerics;
using UnityEngine;

namespace Currencies.Big
{
    [Serializable]
    public class BigPrice
    {
        [Tooltip("Null if IAP.")]
        public BigMoney money;
        public UInt128 amount;

        public BigPrice()
        {
        }

        public BigPrice(BigMoney money, UInt128 amount)
        {
            this.money = money;
            this.amount = amount;
        }

        public bool CanAfford 
            => money.CanSpend(amount);

        public MoneyOperation<UInt128> Apply(bool negative)
            => negative ? Purchase() : Receive();

        public MoneyOperation<UInt128> Purchase()
            => money.Spend(amount);

        public MoneyOperation<UInt128> Receive()
            => money.Add(amount);
        
        public static implicit operator BigMoney(BigPrice price)
            => price.money;
    }
}