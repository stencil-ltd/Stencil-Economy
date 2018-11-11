using JetBrains.Annotations;
using Merch.Data;
using UnityEngine;

namespace Merch.UI
{
    public abstract class MerchDisplay : MonoBehaviour
    {
        [CanBeNull] public MerchItem Item { get; private set; }
        public MerchDisplayConfig Config { get; private set; }

        public void MerchConfigure([CanBeNull] MerchItem item, MerchDisplayConfig config)
        {
            Item = item;
            Config = config;
            OnConfigure();
        }

        protected abstract void OnConfigure();
    }
}