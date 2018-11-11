using Merch.System;
using UnityEngine;
using UnityEngine.UI;

namespace Merch.UI
{
    public abstract class MerchItemView : MonoBehaviour
    {
        public MerchResult Result { get; private set; }
        
        private void Awake()
        {
            GetComponent<Button>()?.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            if (Result.State.Selected)
                MerchSystem.Instance.SetEquipped(Result.Item, true);
            else MerchSystem.Instance.SetSelected(Result.Item);
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