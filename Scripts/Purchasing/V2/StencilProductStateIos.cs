#if UNITY_IOS
using System;
using Analytics;
using Stencil.Economy.Purchasing.Reporting;
using UniRx.Async;
using UnityEngine;
using UnityEngine.Purchasing;

namespace Stencil.Economy.Purchasing
{
    public class StencilProductStateIos : StencilProductState
    {
        public StencilProductStateIos(Product product) : base(product)
        {}

        protected override void Refresh()
        {
            base.Refresh();
            transactionId = product.transactionID;
            receipt = payload;
            signature = null; 
        }

        public override async UniTask ReportSubscriptionPurchase()
        {
            try
            {
                var report = PurchaseReporter.Get(product);
                if (report == null) return;
                var response = await report.ReportIos(receipt, transactionId);
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