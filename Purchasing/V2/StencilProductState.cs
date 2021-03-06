#if UNITY_PURCHASING
using System;
using System.Collections.Generic;
using Analytics;
using JetBrains.Annotations;
using Scripts.RemoteConfig;
using UniRx.Async;
using UnityEngine;
using UnityEngine.Purchasing;

#if STENCIL_TENJIN
using Scripts.Tenjin;
#endif

namespace Stencil.Economy.Purchasing
{
    public class StencilProductState : IStencilProductState
    {
        public static IStencilProductState Get(Product product)
        {
            #if UNITY_EDITOR
            return new StencilProductState(product);
            #elif UNITY_IOS
            return new StencilProductStateIos(product);
            #elif UNITY_ANDROID
            return new StencilProductStateAndroid(product);
            #else
            return new StencilProductState(product);
            #endif
        }

        public Product product { get; }

        protected string productId;
        protected double price;
        protected string currencyCode;

        [CanBeNull] protected Dictionary<string, object> wrapper;
        [CanBeNull] protected string payload;

        // Platform-dependent values.
        [CanBeNull] protected string transactionId;
        [CanBeNull] protected string receipt;
        [CanBeNull] protected string signature;
        
        // Subscription Only.
        [CanBeNull] protected StencilSubscriptionState subscription;

        protected StencilProductState(Product product)
        {
            this.product = product;
        }

        protected virtual void Refresh()
        {
            Debug.Log($"StencilProductState: Refresh {productId}");
            productId = product.definition.id;
            CheckNotNull(productId, "Product ID");
            price = decimal.ToDouble(product.metadata.localizedPrice);
            currencyCode = product.metadata?.isoCurrencyCode;
            if (currencyCode == null)
            {
                currencyCode = "USD";
                var price = product.metadata?.localizedPriceString ?? "UNKNOWN";
                Tracking.Instance
                    .Track("null_currency_code", "product", productId, "price", price)
                    .SetUserProperty("null_currency_code", true);
                Tracking.LogException(new Exception($"Null Currency: {productId} - {price}"));
            }
            if (product.receipt == null)
            {
                Debug.Log($"StencilProductState: No Receipt {productId}");
                wrapper = null;
                payload = null;
                subscription = null;
                return;
            }
            wrapper = (Dictionary<string, object>) MiniJson.JsonDecode(product.receipt);
            payload = (string) wrapper["Payload"];
            if (product.definition.type == ProductType.Subscription)
                subscription = new StencilSubscriptionState(product);
        }

        public void CheckSubscription()
        {
            try
            {
                if (!IsEnabled()) return;
                
                Debug.Log($"StencilProductState: Check Subscription {productId}");
                Refresh();
                
                if (subscription == null)
                    return;

                var info = subscription.info;
                var now = DateTime.UtcNow;
                if (info.isSubscribed() != Result.True || info.isExpired() == Result.True)
                {
                    Debug.LogWarning($"StencilProductState: Not subscribed {productId}");
                    subscription.Clear();
                    return;
                }
                subscription.FirstPurchaseDate = subscription.FirstPurchaseDate ?? now;
                
                if (info.isFreeTrial() == Result.True)
                {
                    Debug.Log($"StencilProductState: Free Trial {productId}");
                    return;
                }
                subscription.FirstChargeDate = subscription.FirstChargeDate ?? now;

                var last = subscription.LastCharge;
                if (last == null)
                {
                    Debug.Log($"StencilProductState: First Charge: {productId}");
                    OnTrackPurchase();
                    subscription.LastCharge = now;
                    return;
                }

                var next = last.Value + subscription.repeatInterval;
                var count = 0;
                while (next.ToUniversalTime() < now.ToUniversalTime())
                {
                    Debug.Log($"StencilProductState: One valid charge {productId}");
                    count++;
                    next += subscription.repeatInterval;

                    if (count > 100)
                    {
                        throw new Exception("Recorded more than 100 renewals of subscription.");
                    }
                }
                if (count > 0)
                {
                    Debug.Log($"StencilProductState: Charge x{count} {productId}");
                    OnTrackPurchase(count);
                    subscription.LastCharge = next;
                }
            }
            catch (Exception e)
            {
                Tracking.LogException(e);
            }
        }

        public virtual UniTask ReportSubscriptionPurchase()
        {
            return UniTask.CompletedTask;
        }

        public void TrackPurchase()
        {
            try
            {
                if (!IsEnabled()) return;
                Debug.Log($"StencilProductState: TrackPurchase {productId}");
                if (product.definition.type == ProductType.Subscription)
                {
                    // submethod will call refresh.
                    Debug.Log($"StencilProductState: TrackPurchase -> CheckSubscription {productId}");
                    CheckSubscription();
                }
                else
                {
                    Debug.Log($"StencilProductState: Valid Consumable Purchase {productId}");
                    Refresh();
                    OnTrackPurchase();
                }
            }
            catch (Exception e)
            {
                Tracking.LogException(e);
            }
        }

        protected bool IsEnabled()
        {
            return true;
        }

        protected virtual void OnTrackPurchase(int count = 1)
        {
            Debug.Log($"StencilProductState: Process Receipt: {productId} {currencyCode} {price} {receipt} {signature}");
            if (!StencilRemote.IsProd()) return;
            #if STENCIL_TENJIN && !UNITY_EDITOR
            StencilTenjin.Instance.tenjin.Transaction(productId, currencyCode, count, price, transactionId, receipt, signature);
            #endif
        }
        
        protected void CheckNotNull(object field, string name)
        {
            if (field == null) throw new NullReferenceException($"Null {name}");
        }
    }
}
#endif