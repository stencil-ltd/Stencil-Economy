using System.Collections.Generic;
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

        public MerchItem Find(string id)
        {
            return _itemMap.ContainsKey(id) ? _itemMap[id] : null;
        }

        public MerchGroup GetGroup(MerchItem item)
        {
            return _itemToGroup.ContainsKey(item) ? _itemToGroup[item] : null;
        }

        public bool IsLocked(MerchItem item)
        {
            var group = GetGroup(item);
            return group != null && group.LockItems && LockedInternal(item);
        }

        public void SetLocked(MerchItem item, bool locked)
        {
            SetLockedInternal(item, locked);
            Save();
        }

        public bool IsAcquired(MerchItem item)
        {
            return AcquiredInternal(item);
        }

        public void SetAcquired(MerchItem item, bool acquired)
        {
            if (IsAcquired(item) == acquired) return;
            SetAcquiredInternal(item, acquired);
            Save();
        }

        public bool IsEquipped(MerchItem item)
        {
            var group = GetGroup(item);
            if (group?.SingleEquip == true)
                return EquippedSingle(group) == item;
            return EquippedMulti(item);
        }

        public void SetEquipped(MerchItem item, bool equipped)
        {
            if (IsEquipped(item) == equipped) return;
            var group = GetGroup(item);
            if (group?.SingleEquip == true)
                SetEquippedSingle(item);
            else
                SetEquippedMulti(item, equipped);
            Save();
        }

        public bool IsSelected(MerchItem item)
        {
            return Selected == item;
        }

        public void SetSelected(MerchItem item)
        {
            if (item == Selected) return;
            Selected = item;
            Notify();
        }

        public void Save()
        {
            PlayerPrefs.Save();
            Notify();
        }

        private void Notify()
        {
            OnChange?.Invoke();
        }
    }
}