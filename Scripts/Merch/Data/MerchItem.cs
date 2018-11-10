using System;
using Common;
using Merch.Data.Properties;
using UnityEngine;
using Util;

namespace Merch.Data
{
    [CreateAssetMenu(menuName = "Economy/Merch Item")]
    public class MerchItem : StencilData
    {
        public bool Unlockable;
        
        public MerchProperties Properties = new MerchProperties();
        
        public event EventHandler OnChange;
        public void NotifyChanged()
        {
            OnChange?.Invoke();
        }
    }
}