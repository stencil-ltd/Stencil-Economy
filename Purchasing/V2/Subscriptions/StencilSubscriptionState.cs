#if UNITY_PURCHASING
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using Scripts.Prefs;
using UnityEngine;
using UnityEngine.Purchasing;
using Util;

namespace Stencil.Economy.Purchasing
{
    public class StencilSubscriptionState
    {
        public readonly string id;
        public readonly Product product;
        public readonly SubscriptionInfo info;

        public readonly TimeSpan freeInterval;
        public readonly TimeSpan repeatInterval;

        public DateTime? FirstPurchaseDate
        {
            get => StencilPrefs.Default.GetDateTime($"stencil_sub_first_purchase_{id}");
            set => StencilPrefs.Default.SetDateTime($"stencil_sub_first_purchase_{id}", value).Save();
        }  
        
        public DateTime? FirstChargeDate
        {
            get => StencilPrefs.Default.GetDateTime($"stencil_sub_first_charge_{id}");
            set => StencilPrefs.Default.SetDateTime($"stencil_sub_first_charge_{id}", value).Save();
        }
        
        public DateTime? LastCharge
        {
            get => StencilPrefs.Default.GetDateTime($"stencil_sub_last_charge_{id}");
            set => StencilPrefs.Default.SetDateTime($"stencil_sub_last_charge_{id}", value).Save();
        }

        public TimeSpan? RecordedRepeatInterval
        {
            get => StencilPrefs.Default.GetTimeSpan($"stencil_sub_repeat_interval_{id}");
            set => StencilPrefs.Default.SetTimeSpan($"stencil_sub_repeat_interval_{id}", value).Save();
        }

        public StencilSubscriptionState(Product product)
        {
            this.product = product;
            id = product.definition.id;
            info = new SubscriptionManager(product, GetIntroJson()).getSubscriptionInfo();
            freeInterval = info.getFreeTrialPeriod();
            repeatInterval = info.getSubscriptionPeriod();
            if (repeatInterval == TimeSpan.Zero)
            {
                var recorded = RecordedRepeatInterval;
                if (recorded != null)
                {
                    repeatInterval = recorded.Value;
                }
                else
                {
                    repeatInterval = info.getExpireDate().ToUniversalTime() - DateTime.UtcNow;
                    if (info.isFreeTrial() == Result.True)
                        repeatInterval -= freeInterval;
                    RecordedRepeatInterval = repeatInterval;
                }
            }
            Debug.Log($"TenjinProduct: {id} {freeInterval.TotalDays} -> {repeatInterval.TotalDays}");
        }

        public void Clear()
        {
            FirstPurchaseDate = null;
            FirstChargeDate = null;
            LastCharge = null;
            RecordedRepeatInterval = null;
        }

        /**
         * https://gist.github.com/AlexSikilinda/20b745bfa21b7f6b9393bc51f7459eaf
         */
        [CanBeNull]
        private string GetIntroJson()
        {
            if (!StencilPlatforms.IsIos(false)) return null;
            var apple = CodelessIAPStoreListener.Instance.ExtensionProvider.GetExtension<IAppleExtensions>();
            Dictionary<string, string> dict = apple.GetIntroductoryPriceDictionary();
            return dict == null || !dict.ContainsKey(product.definition.storeSpecificId) ? null : dict[product.definition.storeSpecificId];
        }
    }
}
#endif