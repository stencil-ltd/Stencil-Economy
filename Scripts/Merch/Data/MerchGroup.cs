using System;
using Common;
using Malee;
using UnityEngine;
using Util;

namespace Merch.Data
{
    [CreateAssetMenu(menuName = "Economy/Merch Group")]
    public class MerchGroup : StencilData
    {
        [Header("Settings")]
        public bool SingleEquip = true;
        public bool LockItems = true;
        
        [Reorderable]
        public GrantArray Grants;
        
        [Reorderable]
        public ListingArray Listings;

        public event EventHandler OnChange;
        public void NotifyChanged()
        {
            OnChange?.Invoke();
        }
    }
}