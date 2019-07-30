using Binding;
using UnityEngine;
using UnityEngine.UI;

namespace Currencies.UI
{
    [RequireComponent(typeof(Text))]
    public class CurrencyCounter : MonoBehaviour
    {
        public Currency Currency;

        public string Prefix = "x";
        
        [Bind]
        private Text _text;

        private void Awake()
        {
            this.Bind(); 
            Currency.OnSpendableChanged += OnChange;
        }

        private void OnEnable()
        {
            MyUpdate();
        }

        private void OnDestroy()
        {
            Currency.OnSpendableChanged -= OnChange;
        }

        private void OnChange(object sender, CurrencyEvent currencyEvent)
        {
            MyUpdate();
        }

        private void MyUpdate()
        {
            _text.text = $"{Prefix}{Currency.Spendable()}";
        }
    }
}