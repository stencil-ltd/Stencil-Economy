

using Stencil.Economy.Purchasing.Reporting;
#if UNITY_ANDROID
using System;
using System.Collections.Generic;
using Analytics;
using UniRx.Async;
using UnityEngine;
using UnityEngine.Purchasing;

namespace Stencil.Economy.Purchasing
{
    public class StencilProductStateAndroid : StencilProductState
    {
        public StencilProductStateAndroid(Product product) : base(product)
        {}

        protected override void Refresh()
        {
            base.Refresh();
            if (payload == null) return;
            var gpDetails = (Dictionary<string, object>)MiniJson.JsonDecode(payload);
            CheckNotNull(gpDetails, "gpDetails");
            receipt = (string)gpDetails["json"];
            CheckNotNull(receipt, "gpJson");
            signature = (string)gpDetails["signature"];
            CheckNotNull(signature, "gpSig");
        }

        public override async UniTask ReportSubscriptionPurchase()
        {
            try
            {
                var report = PurchaseReporter.Get(product);
                if (report == null) return;
                var response = await report.ReportAndroid(receipt, signature);
                if (response?.IsSuccessStatusCode != false)
                {
                    Debug.Log($"Tenjin: Successfully registered purchase with firebase.");                    
                }
                else
                {
                    Debug.LogError($"Tenjin: Could not report to firebase: {response.ReasonPhrase}");
                }
            }
            catch (Exception e)
            {
                Tracking.LogException(e);
            }
        }
    }
}

#endif