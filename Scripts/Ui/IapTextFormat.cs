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
            
        }

        private void LateUpdate()
        {
            var text = _text;
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
            if (changed) text.text = str;
        }
    }
}