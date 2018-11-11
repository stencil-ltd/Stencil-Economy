using Merch.System;
using UnityEngine;

namespace Merch.UI
{
    public abstract class MerchItemView : MonoBehaviour
    {
        public MerchResult Result { get; private set; }
        
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