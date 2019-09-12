using System;
using System.Collections;
using Binding;
using Scripts.Purchasing;
using UnityEngine;
using UnityEngine.Purchasing;

namespace Scripts.Ui
{
    [RequireComponent(typeof(IAPButton))]
    public class IapButtonFormat : MonoBehaviour
    {
        public bool applyHacks = true;
        public string prefix;
        public string suffix;
        
        [Bind] private IAPButton _button;
        
        private void Awake()
        {
            this.Bind();
        }

        private IEnumerator Start()
        {
            yield return null;
            if (applyHacks) _button.ApplyPlatformHacks(false);
        }

        private void LateUpdate()
        {
            var text = _button.priceText;
            if (text == null) return;
            var str = text.text;
            var changed = false;
            if (!str.StartsWith(prefix))
            {
                changed = true;
                str = $"{prefix}{str}";
            }
            if (!str.EndsWith(suffix))
            {
                changed = true;
                str = $"{str}{suffix}";
            }

            if (changed)
            {
                text.text = str;
                if (applyHacks) _button.ApplyPlatformHacks(false);
            }
        }
    }
}