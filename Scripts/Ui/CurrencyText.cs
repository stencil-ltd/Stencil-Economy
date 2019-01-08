using Binding;
using Currencies;
using Texts;
using UnityEngine;

namespace Ui
{
    [RequireComponent(typeof(BigNumberText))]
    public class CurrencyText : MonoBehaviour
    {
        [Header("Data")] 
        public Currency currency;
        
        [Bind]
        private BigNumberText _text;

        private void Awake()
        {
            this.Bind();
            _text = _text ?? gameObject.AddComponent<BigNumberText>();
            _text.Set(currency.Spendable());
        }

        private void OnEnable()
        {
            currency.OnSpendableChanged += OnChange;
        }

        private void OnDisable()
        {
            currency.OnSpendableChanged -= OnChange;
        }
        
        private void OnChange(object sender, Currency e)
        {
            _text.Lerp(e.Spendable());
        }
    }
}