using System;
using System.Collections;
using Binding;
using Currencies.Big;
using Dirichlet.Numerics;
using JetBrains.Annotations;
using Lobbing;
using Scripts.Util;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace Currencies.UI
{
    [RequireComponent(typeof(Button))]
    public class BigMoneyButton : MonoBehaviour
    {
        [Header("Configure")]
        public BigPrice price;
        public bool disableOnFail;
        public NumberFormats.Format numberFormat;

        [Header("UI")] 
        public Text amountText;
        public BigPriceEvent success;
        public BigPriceEvent failure;
        
        [Bind] 
        public Button button { get; private set; }

        [Bind] 
        private CanvasGroup _canvasGroup;

        public UInt128 Amount
        {
            get => price.amount;
            set => SetAmount(value);
        }

        protected virtual void Awake()
        {
            this.Bind();
            button = button ?? GetComponent<Button>();
            button.onClick.AddListener(Purchase);
        }

        private void Update()
        {
            var canAfford = price.CanAfford;
            if (disableOnFail)
                button.enabled = canAfford; 
            if (_canvasGroup != null) 
                _canvasGroup.alpha = canAfford ? 1 : 0.5f;
        }

        private void Purchase()
        {
            if (price.amount == 0) return;
            if (price.Purchase().AndSave())
                Objects.StartCoroutine(Success());
            else failure?.Invoke(price);
        }

        private IEnumerator Success()
        {
            success?.Invoke(price);
            yield break;
        }

        public void SetAmount(UInt128 amount)
        {
            price.amount = amount;
            RefreshUi();
        }

        public void SetPrice(BigPrice price)
        {
            this.price = price;
            RefreshUi();
        }

        private void RefreshUi()
        {
            if (amountText)
                amountText.text = $"x{Format(price)}";
        }

        protected virtual string Format(BigPrice price)
            => numberFormat.FormatAmount(price.amount);
    }
}