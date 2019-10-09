using System;
using Analytics;
using Currencies;
using Dirichlet.Numerics;
using Scripts.Prefs;
using UI;
using UnityEngine;
using UnityEngine.Purchasing;
using Stencil.Economy.Purchasing;

namespace Scripts.Purchasing
{
    public class BasicPurchaseHandler : Controller<BasicPurchaseHandler>
    {
#if UNITY_PURCHASING
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
        public static event EventHandler<Product> OnPurchase;
        public static event EventHandler<PurchaseFailureReason> OnPurchaseFail;
        
        private IAPListener _listener;

        private void Start()
        {
            _listener = gameObject.AddComponent<IAPListener>();
            _listener.dontDestroyOnLoad = false;
            
            _listener.onPurchaseComplete = _listener.onPurchaseComplete ?? new IAPListener.OnPurchaseCompletedEvent();
            _listener.onPurchaseComplete.AddListener(_OnPurchase);
            _listener.onPurchaseFailed = _listener.onPurchaseFailed ?? new IAPListener.OnPurchaseFailedEvent();
            _listener.onPurchaseFailed.AddListener(_OnFailure);
        }

        private void OnDestroy()
        {
            if (_listener == null) return;
            _listener.onPurchaseComplete.RemoveListener(_OnPurchase);
            _listener.onPurchaseFailed.RemoveListener(_OnFailure);
        }

        private void _OnPurchase(Product product)
        {
            Debug.Log($"Purchasing product {product.definition.id}");
            Tracking.Instance.Track("iap_purchase", "product", product.definition.id);
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
            OnPurchase?.Invoke(this, product);
            
            Count++;
            Last = DateTime.UtcNow;

            try
            {
                StencilProductState.Get(product).TrackPurchase();
            }
            catch (Exception e)
            {
                Tracking.LogException(e);
            }
        }

        private void _OnFailure(Product product, PurchaseFailureReason reason)
        {
            Debug.LogError($"Failed to purchase product {product.definition.id}");
            OnPurchaseFail?.Invoke(product, reason);
        }
#endif
    }
}