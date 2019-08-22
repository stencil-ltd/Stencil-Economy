using System;
using Analytics;
using Scripts.Payouts;
using Scripts.Prefs;
using Scripts.Purchasing;
using Scripts.Tenjin.Abstraction;
using UI;
using UniRx.Async;
using UnityEngine;
using UnityEngine.Purchasing;
using Util;
using Price = Currencies.Price;

namespace Stencil.Subscriptions
{
    public class StencilSubscriptions : Controller<StencilSubscriptions>
    {
        public static bool IsSubscribed
        {
            get => StencilPrefs.Default.GetBool("subscription_premium");
            set => StencilPrefs.Default.SetBool("subscription_premium", value).Save();
        }
        public static event EventHandler OnStateChanged;
        public static event EventHandler<Price> OnPayout; 

        [Header("Settings")]
        public string key = "subscription_coins";
        public string productId;
        public Product product;
        
        [Header("Timing")]
        [Tooltip("How many days' payout should be stored up in player's absence?")]
        public int maxDaysPayout = 7;
        [Tooltip("Should payout occur on calendar day? (can be combined with useIntervalBoundary")]
        public bool useDayBoundary = true;
        [Tooltip("Should payout occur every interval period? (can be combined with useIntervalBoundary")]
        public bool useIntervalBoundary = true;
        
        private DailyPayout payout;

        private void Start()
        {
            payout = new DailyPayout(key)
            {
                maxMult = maxDaysPayout,
                useDayBoundary = useDayBoundary,
                useIntervalBoundary = useIntervalBoundary
            };
            OnStateChanged?.Invoke();
            var _ = Refresh();
        }

        public async UniTask Refresh()
        {
            Debug.Log("StencilSubscriptions: Refresh...");
            var start = DateTime.UtcNow;
            while (!CodelessIAPStoreListener.initializationComplete)
            {
                await UniTask.Yield();
                var now = DateTime.UtcNow;
                if ((now - start).Seconds >= 10)
                {
                    Debug.LogError("Subscriptions: Not ready after 10 seconds.");
                    return;
                }
            }
            
            Debug.Log("StencilSubscriptions: IAP Ready.");
            var product = CodelessIAPStoreListener.Instance.GetProduct(productId);
            if (product == null)
            {
                Tracking.LogException(new NullReferenceException("Can't find subscription product"));
                return;
            }
            
            IsSubscribed = product.hasReceipt;
            Debug.Log($"StencilSubscriptions: {IsSubscribed}");
            HandlePayout(product);
            OnStateChanged?.Invoke();
        }

        private void HandlePayout(Product product)
        {
            Debug.Log($"StencilSubscriptions: Check Daily Payout");
            try
            {
                TenjinProduct.Get(product).CheckSubscription();
                if (IsSubscribed)
                { 
                    TryPayout(product);
                }
                else
                {
                    Debug.Log($"StencilSubscriptions: Payout state cleared");
                    payout.Clear();
                }
            }
            catch (Exception e)
            {
                Debug.LogError("StencilSubscriptions: Exception Raised");
                Tracking.LogException(e);
            }
        }

        private void TryPayout(Product product)
        {
            var payouts = product.GetPayouts();
            var peek = payout.Peek();
            if (peek == 0) return;
            foreach (var price in payouts)
            {
                price.Currency?.Add(price.GetAmount() * (uint) peek);
                OnPayout?.Invoke(this, price);
            }
            payout.Mark();
        }
    }
}