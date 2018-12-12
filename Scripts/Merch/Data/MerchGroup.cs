using System;
using System.Collections.Generic;
using Merch.Data.Properties;
using State;
using UnityEngine;
using Util;

namespace Merch.Data
{
    [CreateAssetMenu(menuName = "Economy/Merch Group")]
    public class MerchGroup : StencilData
    {
        [Header("Settings")]
        public bool SingleEquip = true;
        public bool RequiredEquip = false;
        public bool Lockable = true;

        public MerchProperties Properties = new MerchProperties();
        
        public List<MerchGrant> Grants;
        
        public List<MerchListing> Listings;

        public event EventHandler OnChange;
        public void NotifyChanged()
        {
            OnChange?.Invoke();
        }
    }
}