using System;
using Binding;
using Currencies;
using Lobbing;
using Scripts.Purchasing;
using UnityEngine;

namespace Stencil.Economy.Ui
{
    [RequireComponent(typeof(Lobber))]
    public class GenericCurrencyLobber : MonoBehaviour
    {
#if UNITY_PURCHASING

        public Currency currency;
        
        [Bind] private Lobber _lobber;

        private void Awake()
        {
            this.Bind();
        }

        private void OnEnable()
        {
            BasicPurchaseHandler.OnConsumable += _OnPurchase;
        }

        private void OnDisable()
        {
            BasicPurchaseHandler.OnConsumable -= _OnPurchase;
        }

        private void _OnPurchase(object sender, Price e)
        {
            if (e.Currency == currency)
                StartCoroutine(_lobber.LobMany((ulong) e.GetAmount()));
        }
#endif
    }
}