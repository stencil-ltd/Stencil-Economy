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
        public event EventHandler OnChange;
        
        public MerchProperties Properties = new MerchProperties();

        internal void NotifyChange()
        {
            OnChange?.Invoke();
        }
    }
}