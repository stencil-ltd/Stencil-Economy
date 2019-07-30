using Binding;
using UnityEngine;
using UnityEngine.UI;

namespace Currencies.UI
{
    [RequireComponent(typeof(Image))]
    public class CurrencyIcon : MonoBehaviour
    {
        public Currency Currency;

        [Bind]
        private Image _image;

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
            _image.sprite = Currency.BestSprite();   
        }
    }
}