using System;
using System.Collections;
using Binding;
using Scripts.Purchasing;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

namespace Scripts.Ui
{
    [RequireComponent(typeof(Text))]
    public class IapTextFormat : MonoBehaviour
    {
        public string productId;
        public string prefix;
        public string suffix;

        [Bind] private Text _text;

        private void Awake()
        {
            this.Bind();
        }

        private IEnumerator Start()
        {
            while (!CodelessIAPStoreListener.initializationComplete) yield return null;
            var product = CodelessIAPStoreListener.Instance.GetProduct(productId);
            var str = prefix + product.metadata.localizedPriceString + suffix;
            _text.text = str;
        }
    }
}