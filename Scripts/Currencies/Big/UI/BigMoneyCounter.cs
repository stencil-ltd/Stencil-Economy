using System;
using Binding;
using Currencies.Big;
using Dirichlet.Numerics;
using JetBrains.Annotations;
using Lobbing;
using Scripts.Util;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace Currencies
{
    public class BigMoneyCounter : MonoBehaviour
    {
        [Serializable]
        public enum CurrencyDisplay
        {
            Spendable,
            Total,
            Staged
        }
        
        [Header("Config")]
        public BigMoney money;
        public float speed = 5f;
        public bool onlyCommitAmount;
        public CurrencyDisplay display = CurrencyDisplay.Spendable;

        [Header("Format")]
        public string String = "x{0}";
        public string format = "N0";
        public NumberFormats.Format customFormatter;

        [Header("UI")] 
        public Image icon;
        public Text text;
        
        private Coroutine _co;

        private void Awake()
        {
            this.Bind();
            if (icon && money)
            {
                icon.sprite = money.Sprite();
                icon.SetNativeSize();
            }
        }

        private void OnEnable()
        {
            if (money != null)
            {
                switch (display)
                {
                    case CurrencyDisplay.Spendable:
                        money.OnSpendableChanged += OnChange;
                        break;
                    case CurrencyDisplay.Total:
                    case CurrencyDisplay.Staged:
                        money.OnTotalChanged += OnChange;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            UpdateText();
        }

        private void OnDisable()
        {
            if (money != null)
            {
                money.OnSpendableChanged -= OnChange;
                money.OnTotalChanged -= OnChange;
            }
        }

        private UInt128 Amount
        {
            get
            {
                if (money == null) return 0L;
                switch (display)
                {
                    case CurrencyDisplay.Spendable:
                        return money.Spendable();
                    case CurrencyDisplay.Total:
                        return money.Total();
                    case CurrencyDisplay.Staged:
                        return money.Staged();
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void OnChange(object sender, IMoney<UInt128> money)
        {
            if (_co != null) StopCoroutine(_co);
            
            if (customFormatter == NumberFormats.Format.None)
                _co = StartCoroutine(text.LerpAmount(String, format, Amount, 1f));
            else 
                _co = StartCoroutine(text.LerpAmount(String, customFormatter, Amount, 1f));
        }

        private void UpdateText()
        {
            if (customFormatter == NumberFormats.Format.None)
                text.SetAmount(String, format, Amount);
            else 
                text.SetAmount(String, customFormatter, Amount);
        }
    }
}
