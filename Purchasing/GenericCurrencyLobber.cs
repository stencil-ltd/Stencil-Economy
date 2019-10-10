using System;
using System.Collections;
using System.Collections.Generic;
using Binding;
using Currencies;
using Lobbing;
using Purchasing.Lobbing;
using Scripts.Purchasing;
using Sirenix.Utilities;
using UnityEngine;

namespace Stencil.Economy.Ui
{
    [RequireComponent(typeof(Lobber))]
    public class GenericCurrencyLobber : MonoBehaviour
    {
        public static int LobCount { get; private set; }
        
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
            currency.OnSpendableChanged += _OnSpend;
        }

        private void OnDisable()
        {
            BasicPurchaseHandler.OnConsumable -= _OnPurchase;
            currency.OnSpendableChanged -= _OnSpend;
        }

        private void _OnSpend(object sender, CurrencyEvent e)
        {
            if (e.currency != currency) return;
            if (e.to >= e.from) return;
            StartCoroutine(_Lob(e.currency, (ulong) (e.from - e.to), true));
        }

        private void _OnPurchase(object sender, Price e)
        {
            if (e.Currency != currency) return; 
            StartCoroutine(_Lob(e.Currency, (ulong) e.GetAmount(), false));
        }

        private IEnumerator _Lob(Currency currency, ulong amount, bool spend)
        {
            LobCount++;
            Debug.Log($"Begin Lob: {currency}");
            
            // Lob to from/to combinations.
            var froms = CurrencyLobTarget.GetTargets(currency, LobTargetType.From, spend);
            var tos = CurrencyLobTarget.GetTargets(currency, LobTargetType.To, spend);
            var lobs = new List<Coroutine>();
            
            foreach (var from in froms)
            {
                foreach (var to in tos)
                {
                    var overrides = new LobOverrides
                    {
                        From = from.transform,
                        To = to.transform
                    };
                    lobs.Add(StartCoroutine(_lobber.LobMany(amount, overrides)));
                }
            }
            
            if (_lobber.From != null && _lobber.To != null)
                lobs.Add(StartCoroutine(_lobber.LobMany(amount)));
            
            foreach (var coroutine in lobs)
                yield return coroutine;
            
            Debug.Log($"Finish Lob: {currency}");
            LobCount--;
        }
#endif
    }
}