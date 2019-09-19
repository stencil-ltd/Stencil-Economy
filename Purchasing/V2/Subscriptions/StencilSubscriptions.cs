using System;
using System.Collections;
using Analytics;
using Currencies;
using Scripts.Payouts;
using Scripts.Prefs;
using Scripts.Purchasing;
using UI;
using UnityEngine;
using UnityEngine.Purchasing;
using Util;
using Price = Currencies.Price;

namespace Stencil.Economy.Purchasing.Subscriptions
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
        public string key = "standard_subscription";
        public string productId;
        public Product product;
        
        [Header("Timing")]
        [Tooltip("How many days' payout should be stored up in player's absence?")]
        public int maxDaysPayout = 7;
        [Tooltip("Should payout occur on calendar day? (can be combined with useIntervalBoundary")]
        public bool useDayBoundary = true;
        [Tooltip("Should payout occur every interval period? (can be combined with useIntervalBoundary")]
        public bool useIntervalBoundary = true;
        public bool delayBetweenPayouts = true;
        
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
            Objects.StartCoroutine(Refresh());
            BasicPurchaseHandler.OnPurchase += _OnPurchase;
        }

        private void OnDestroy()
        {
            BasicPurchaseHandler.OnPurchase -= _OnPurchase;
        }

        private void _OnPurchase(object sender, Product e)
        {
            StartCoroutine(Refresh());
        }

        public IEnumerator Refresh()
        {
            Debug.Log("StencilSubscriptions: Refresh...");
            var start = DateTime.UtcNow;
            while (!CodelessIAPStoreListener.initializationComplete)
            {
                yield return null;
                var now = DateTime.UtcNow;
                if ((now - start).Seconds >= 10)
                {
                    Debug.LogError("Subscriptions: Not ready after 10 seconds.");
                    yield break;
                }
            }
            
            Debug.Log("StencilSubscriptions: IAP Ready.");
            var product = CodelessIAPStoreListener.Instance.GetProduct(productId);
            if (product == null)
            {
                Tracking.LogException(new NullReferenceException("Can't find subscription product"));
                yield break;
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
                StencilProductState.Get(product).CheckSubscription();
                if (IsSubscribed)
                { 
                    Objects.StartCoroutine(TryPayout(product));
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

        private IEnumerator TryPayout(Product product)
        {
            var payouts = product.GetPayouts();
            var peek = payout.Peek();
            if (peek == 0) yield break;
            foreach (var price in payouts)
            {
                var currency = price.Currency;
                if (currency == null) continue;
                currency.Add(price.GetAmount() * (uint) peek);
                OnPayout?.Invoke(this, price);
                if (delayBetweenPayouts) yield return new WaitForSeconds(1f);
            }
            payout.Mark(); // this should also save out the currency states.
        }
    }
}