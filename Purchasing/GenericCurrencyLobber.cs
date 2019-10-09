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
        public static Coroutine LatestLob { get; private set; }
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
        }

        private void OnDisable()
        {
            BasicPurchaseHandler.OnConsumable -= _OnPurchase;
        }

        private void _OnPurchase(object sender, Price e)
        {
            if (e.Currency == currency) 
                LatestLob = StartCoroutine(Lob(e));
        }

        private IEnumerator Lob(Price e)
        {
            var co = StartCoroutine(_Lob(e));
            LatestLob = co;
            yield return co;
            if (LatestLob == co) LatestLob = null;
        }

        private IEnumerator _Lob(Price e)
        {
            Debug.Log($"Begin Lob: {e.Currency}");
            LobCount++;
            var amount = (ulong) e.GetAmount();
            
            // Lob to from/to combinations.
            var froms = CurrencyLobTarget.GetTargets(e.Currency, LobTargetType.From);
            var tos = CurrencyLobTarget.GetTargets(e.Currency, LobTargetType.To);
            if (froms.IsNullOrEmpty() || tos.IsNullOrEmpty()) yield break;

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
            
            Debug.Log($"Finish Lob: {e.Currency}");
            LobCount--;
        }
#endif
    }
}