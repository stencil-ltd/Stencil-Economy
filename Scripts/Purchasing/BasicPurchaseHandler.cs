using System;
using Currencies;
using Dirichlet.Numerics;
using Scripts.Prefs;
using Scripts.Tenjin.Abstraction;
using UI;
using UnityEngine;
using UnityEngine.Purchasing;
using Price = UnityEngine.Purchasing.Price;

namespace Scripts.Purchasing
{
    public class BasicPurchaseHandler : Controller<BasicPurchaseHandler>
    {
        public static DateTime? Last
        {
            get => StencilPrefs.Default.GetDateTime("iap_purchase_last");
            set => StencilPrefs.Default.SetDateTime("iap_purchase_last", value).Save();
        }
        
        public static int Count
        {
            get => StencilPrefs.Default.GetInt("iap_purchase_count");
            private set => StencilPrefs.Default.SetInt("iap_purchase_count", value).Save();
        }
        
        public static event EventHandler<Currencies.Price> OnConsumable;
        private IAPListener _listener;
        
        public override void Register()
        {
            base.Register();
            _listener = gameObject.AddComponent<IAPListener>();
            _listener.dontDestroyOnLoad = false;
            
            _listener.onPurchaseComplete = _listener.onPurchaseComplete ?? new IAPListener.OnPurchaseCompletedEvent();
            _listener.onPurchaseComplete.AddListener(_OnPurchase);
            _listener.onPurchaseFailed = _listener.onPurchaseFailed ?? new IAPListener.OnPurchaseFailedEvent();
            _listener.onPurchaseFailed.AddListener(_OnFailure);
        }
        
        private void _OnPurchase(Product product)
        {
            Debug.Log($"Purchasing product {product.definition.id}");
            if (product.definition.type != ProductType.Subscription)
            {
                foreach (var payout in product.definition.payouts)
                {
                    if (payout.type == PayoutType.Other) continue;
                    var currency = CurrencyManager.Instance.TryGet(payout.subtype);
                    if (currency != null)
                    {
                        var amount = (UInt128) payout.quantity;
                        currency.Add(amount);
                        OnConsumable?.Invoke(this, new Currencies.Price(currency, amount));
                    }
                    else
                    {
                        Debug.LogError($"Could not find currency {payout.subtype}");
                    }
                }
                
            }

            CurrencyManager.Instance.Save();
            TenjinProduct.Get(product).TrackPurchase();
            
            Count++;
            Last = DateTime.UtcNow;
        }

        private void _OnFailure(Product product, PurchaseFailureReason reason)
        {
            Debug.LogError($"Failed to purchase product {product.definition.id}");
        }
        
    }
}