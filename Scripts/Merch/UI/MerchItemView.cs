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
        
        public MerchResult Result { get; private set; }
        
        private void Awake()
        {
            GetComponent<Button>()?.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            if (!Result.State.Selected)
                MerchSystem.Instance.SetSelected(Result.Item);
            else if (!Result.State.Acquired && AcquireOnClick && StencilRemote.IsDeveloper())
                MerchSystem.Instance.SetAcquired(Result.Item, true);
            else MerchSystem.Instance.SetEquipped(Result.Item, true);
        }
        
        public void SetResult(MerchResult result)
        {
            Result = result;
            Refresh();
        }

        public void Refresh()
        {
            OnRefresh();
        }

        protected abstract void OnRefresh();
    }
}