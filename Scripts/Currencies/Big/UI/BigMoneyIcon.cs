using Binding;
using Currencies.Big;
using Dirichlet.Numerics;
using UnityEngine;
using UnityEngine.UI;

namespace Currencies.UI
{
    [RequireComponent(typeof(Image))]
    public class BigMoneyIcon : MonoBehaviour
    {
        public BigMoney money;

        [Bind]
        private Image _image;

        private void Awake()
        {
            this.Bind();
            money.OnSpendableChanged += OnChange;
        }

        private void OnEnable()
        {
            MyUpdate();
        }

        private void OnDestroy()
        {
            money.OnSpendableChanged -= OnChange;
        }

        private void OnChange(object sender, IMoney<UInt128> money1)
        {
            MyUpdate();
        }

        private void MyUpdate()
        {
            _image.sprite = money.Sprite();   
        }
    }
}