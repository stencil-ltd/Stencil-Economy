using System.Collections.Generic;
using Merch.Data;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.U2D;
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
            var group = GetGroup(item);
            if (group?.LockItems == false) return;
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
            var group = GetGroup(item);
            if (group?.SingleEquip == true)
                return EquippedSingle(group) == item;
            return EquippedMulti(item);
        }

        public void SetEquipped(MerchItem item, bool equipped)
        {
            if (IsEquipped(item) == equipped) return;
            var group = GetGroup(item);
            MerchItem old = null;
            if (group?.SingleEquip == true)
            {
                old = EquippedSingle(group);
                SetEquippedSingle(equipped ? item : null);
            }
            else
                SetEquippedMulti(item, equipped);
            Save(item, old);
        }

        public bool IsSelected(MerchItem item)
        {
            return Selected == item;
        }

        public void SetSelected(MerchItem item)
        {
            if (item == Selected) return;
            var old = Selected;
            Selected = item;
            Notify(item, old);
        }

        private void Save(params MerchItem[] items)
        {
            if (_skipSave) return;
            PlayerPrefs.Save();
            Notify(items);
        }
        
        public void Save()
        {
            Save(null);
        }

        private HashSet<MerchGroup> _notifySeenGroups = new HashSet<MerchGroup>();
        private void Notify(params MerchItem[] items)
        {
            if (items != null)
            {
                _notifySeenGroups.Clear();
                foreach (var item in items)
                {
                    if (item == null) continue;
                    item.NotifyChanged();
                    var group = GetGroup(item);
                    if (group != null && !_notifySeenGroups.Contains(group))
                    {
                        _notifySeenGroups.Add(group);
                        group.NotifyChanged();
                    }
                }
            }
            OnChange?.Invoke();
        }
    }
}