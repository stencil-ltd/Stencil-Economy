using System.Runtime.CompilerServices;
using Merch.System;
using Scripts.RemoteConfig;
using UnityEngine;
using UnityEngine.UI;

namespace Merch.UI
{
    public abstract class MerchItemView : MonoBehaviour
    {
        [Header("Debug")] 
        public bool AcquireOnClick;
        public bool UpdateInEditor;
        public bool AllowRotation = true;
        
        public MerchResult Result { get; private set; }
        
        private void Awake()
        {
            GetComponent<Button>()?.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            if (!Result.State.Selected)
                MerchSystem.Instance.SetSelected(Result.Item);
            else if (Result.State.Equipped)
                MerchSystem.Instance.SetEquipped(Result.Item, false);
            else if (Result.State.Acquired && !Result.State.Equipped)
                MerchSystem.Instance.SetEquipped(Result.Item, true);
            else if (AcquireOnClick && StencilRemote.IsDeveloper())
                MerchSystem.Instance.SetAcquired(Result.Item, true);
        }
        
        public void SetResult(MerchResult result)
        {
            Result = result;
            Refresh();
        }

        public void Refresh()
        {
            OnRefresh();
            UpdatePreset();
        }
        
#if UNITY_EDITOR
        private void Update()
        {
            if (UpdateInEditor)
                UpdatePreset();
        }
#endif

        protected abstract void OnRefresh();
        protected virtual void UpdatePreset() {}
    }
}