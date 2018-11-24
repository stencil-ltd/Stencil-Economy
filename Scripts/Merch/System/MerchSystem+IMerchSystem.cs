using System.Collections.Generic;
using System.Linq;
using Merch.Data;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.U2D;
using Util;
using NotImplementedException = System.NotImplementedException;

namespace Merch.System
{
    public partial class MerchSystem
    {
        public MerchState GetState(MerchItem item)
        {
            MerchListing listing;
            var hasListings = _itemToListing.TryGetValue(item, out listing);
            return new MerchState
            {
                Locked = IsLocked(item),
                Selected = IsSelected(item),
                Acquired = IsAcquired(item),
                Equipped = IsEquipped(item),
                MainPrice = hasListings ? listing.MainPrice : null,
                ExtraPrices = hasListings ? listing.ExtraPrices : null
            };
        }

        public MerchItem Find(string id)
        {
            return _itemMap.ContainsKey(id) ? _itemMap[id] : null;
        }

        public MerchGroup GetGroup(MerchItem item)
        {
            return item.Group;
        }

        public IEnumerable<MerchItem> GetItems(MerchGroup group)
        {
            var grants = group.Grants.Select(grant => grant.Item);
            var listings = group.Listings.Select(listing => listing.Item);
            return grants.Concat(listings);
        }

        public bool IsLocked(MerchItem item)
        {
            var group = GetGroup(item);
            return group != null && group.Lockable && LockedInternal(item);
        }

        public void SetLocked(MerchItem item, bool locked)
        {
            var group = GetGroup(item);
            if (group?.Lockable == false) return;
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
            if (!IsAcquired(item)) return;
            var group = GetGroup(item);
            if (group?.SingleEquip == true)
            {
                if (equipped)
                {
                    var old = EquippedSingle(group);
                    SetEquippedSingle(item);
                    Save(item, old);
                }
                else
                {
                    if (group.RequiredEquip)
                    {
                        Debug.LogWarning("Refused to unequip required item");
                        return;
                    }
                    RemoveEquippedSingle(group);
                    Save(item);
                }
            }
            else
            {
                SetEquippedMulti(item, equipped);
                Save(item);
            }
        }

        public MerchItem GetEquippedSingle(MerchGroup group)
        {
            return !group.SingleEquip ? null : EquippedSingle(group);
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