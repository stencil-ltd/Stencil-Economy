using System;
using System.Collections.Generic;
using Common;
using Malee;
using Store;
using UnityEngine;
using Util;

namespace Merch.Data
{
    [CreateAssetMenu(menuName = "Economy/Merch Group")]
    public class MerchGroup : StencilData
    {
        private static Dictionary<CarStoreStates.State, MerchGroup> _stateMap =
            new Dictionary<CarStoreStates.State, MerchGroup>();

        [Header("Settings")]
        public bool SingleEquip = true;
        public bool LockItems = true;
        public CarStoreStates.State UiState;
        
        [Reorderable]
        public GrantArray Grants;
        
        [Reorderable]
        public ListingArray Listings;

        public event EventHandler OnChange;
        public void NotifyChanged()
        {
            OnChange?.Invoke();
        }

        public static MerchGroup ForState(CarStoreStates.State state)
            => _stateMap[state];

        private void OnEnable()
        {
            _stateMap[UiState] = this;
        }
    }
}