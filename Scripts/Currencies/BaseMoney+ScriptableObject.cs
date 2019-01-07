using Plugins.Data;
using Scripts.RemoteConfig;
using UnityEngine;

namespace Currencies
{
    public abstract partial class BaseMoney<T>
    {
        private void OnEnable()
        {
            if (!Application.isPlaying) return;
            this.BindRemoteConfig();
            InitializeData(false);
            ResetButton.OnGlobalReset += (sender, args) => Clear();
        }
    }
}