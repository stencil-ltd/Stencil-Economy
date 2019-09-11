using System;
using System.Collections;
using Binding;
using Scripts.Purchasing;
using UnityEngine;
using UnityEngine.Purchasing;

namespace Scripts.Ui
{
    [RequireComponent(typeof(IAPButton))]
    public class IapHacks : MonoBehaviour
    {
        [Bind] private IAPButton _button;

        private void Awake()
        {
            this.Bind();
        }

        private IEnumerator Start()
        {
            yield return null;
            _button.ApplyPlatformHacks(false);
        }
    }
}