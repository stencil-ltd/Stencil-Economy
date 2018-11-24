using System;
using Common;
using JetBrains.Annotations;
using Merch.Data.Properties;
using Merch.UI;
using State;
using UnityEngine;
using Util;

namespace Merch.Data
{
    [CreateAssetMenu(menuName = "Economy/Merch Item")]
    public class MerchItem : StencilData
    {
        public MerchProperties Properties = new MerchProperties();

        [NonSerialized] 
        public MerchGroup Group;

        [CanBeNull] public MerchDisplayPreset GetBestPreset()
            => Properties?.DisplayPreset ?? Group.Properties.DisplayPreset;
        
        public event EventHandler OnChange;
        public void NotifyChanged()
        {
            OnChange?.Invoke();
        }
    }
}