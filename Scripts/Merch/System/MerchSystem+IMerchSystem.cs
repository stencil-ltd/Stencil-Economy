using System.Collections.Generic;
using JetBrains.Annotations;
using Merch.Data;
using UnityEngine;
using Util;

namespace Merch.System
{
    public partial class MerchSystem
    {
        public MerchState GetState(MerchItem item)
        {
            List<MerchListing> listings;
            var hasListings = _itemToListings.TryGetValue(item, out listings); 
            if (hasListings && listings.Count > 0)
                Debug.LogError($"Found more than one listing for {item}");
            
            return new MerchState
            {
                Locked = IsLocked(item),
                Selected = IsSelected(item),
                Acquired = IsAcquired(item),
                Equipped = IsEquipped(item),
                MainPrice = hasListings ? listings[0].MainPrice : null,
                ExtraPrices = hasListings ? listings[0].ExtraPrices : null
            };
        }

        public MerchGroup GetGroup(MerchItem item)
        {
            return _itemToGroup[item];
        }

        public bool IsLocked(MerchItem item)
        {
            var group = GetGroup(item);
            return group.LockItems && LockedInternal(item);
        }

        public void SetLocked(MerchItem item, bool locked)
        {
            SetLockedInternal(item, locked);
            Save(item);
        }

        public bool IsAcquired(MerchItem item)
        {
            return AcquiredInternal(item);
        }

        public void SetAcquired(MerchItem item, bool acquired)
        {
            if (IsAcquired(item) == acquired) return;
            SetAcquiredInternal(item, acquired);
            Save(item);
        }

        public bool IsEquipped(MerchItem item)
        {
            return EquippedInternal(item);
        }

        public void SetEquipped(MerchItem item, bool equipped)
        {
            if (IsEquipped(item) == equipped) return;
            SetEquippedInternal(item, equipped);
            Save(item);
        }

        public bool IsSelected(MerchItem item)
        {
            return Selected == item;
        }

        public void SetSelected(MerchItem item)
        {
            if (item == Selected) return;
            Selected = item;
            Notify(item);
        }

        public void Save()
        {
            Save(null);
        }

        private void Save([CanBeNull] MerchItem item)
        {
            PlayerPrefs.Save();
            Notify(item);
        }

        private void Notify(MerchItem item = null)
        {
            OnChange?.Invoke();
            item?.NotifyChange();
        }
    }
}