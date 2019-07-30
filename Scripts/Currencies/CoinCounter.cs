using System;
using Binding;
using Dirichlet.Numerics;
using JetBrains.Annotations;
using Lobbing;
using Scripts.Util;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace Currencies
{
    public class CoinCounter : MonoBehaviour
    {
        [Serializable]
        public enum CurrencyDisplay
        {
            Spendable,
            Total,
            Staged
        }
        
        [Header("Config")]
        public Currency Currency;
        public float Speed = 5f;
        public bool OnlyCommitAmount;
        public CurrencyDisplay Display = CurrencyDisplay.Spendable;

        [Header("Format")]
        public string String = "{0}";
        public string Format = "N0";
        public NumberFormats.Format CustomFormatter;

        [Header("UI")] 
        public Image Icon;
        public Text Text;
        
        [CanBeNull]
        [Bind]
        public Lobber Lobber { get; private set; }
        
        private Coroutine _co;
        
        private void Awake()
        {
            this.Bind();
            if (Icon && Currency)
            {
                Icon.sprite = Currency.BestSprite();
                Icon.SetNativeSize();
            }
        }

        private void OnEnable()
        {
            if (Currency != null)
            {
                switch (Display)
                {
                    case CurrencyDisplay.Spendable:
                        Currency.OnSpendableChanged += OnChange;
                        break;
                    case CurrencyDisplay.Total:
                    case CurrencyDisplay.Staged:
                        Currency.OnTotalChanged += OnChange;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            UpdateText();
        }

        private void OnDisable()
        {
            if (Currency != null)
            {
                Currency.OnSpendableChanged -= OnChange;
                Currency.OnTotalChanged -= OnChange;
            }
        }

        private void OnChange(object sender, CurrencyEvent currencyEvent)
        {           
            if (_co != null) StopCoroutine(_co);
            _co = StartCoroutine(Text.LerpAmount(String, CustomFormatter, Amount, 1f));
        }

        private UInt128 Amount
        {
            get
            {
                if (Currency == null) return 0L;
                switch (Display)
                {
                    case CurrencyDisplay.Spendable:
                        return Currency.Spendable();
                    case CurrencyDisplay.Total:
                        return Currency.Total();
                    case CurrencyDisplay.Staged:
                        return Currency.Staged();
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void UpdateText()
        {
            Text.SetAmount(String, CustomFormatter, Amount);
        }
        
        [CanBeNull]
        public static implicit operator Lobber(CoinCounter counter)
            => counter.Lobber;
    }
}
