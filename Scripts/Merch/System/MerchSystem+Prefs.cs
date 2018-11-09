using JetBrains.Annotations;
using Merch.Data;
using UnityEngine;
using Util;
// ReSharper disable SuggestBaseTypeForParameter

namespace Merch.System
{
    public partial class MerchSystem
    {
        private static string GetKey(string name, string id)
            => $"merch_{name}_{id}";
        
        private bool LockedInternal(MerchItem item) 
            => PlayerPrefsX.GetBool(GetKey("locked", item.Id), true);
        private void SetLockedInternal(MerchItem item, bool locked)
            => PlayerPrefsX.SetBool(GetKey("locked", item.Id), locked);
        
        private bool AcquiredInternal(MerchItem item) 
            => PlayerPrefsX.GetBool(GetKey("acquired", item.Id));
        private void SetAcquiredInternal(MerchItem item, bool locked)
            => PlayerPrefsX.SetBool(GetKey("acquired", item.Id), locked);
        
        private bool EquippedMulti(MerchItem item) 
            => PlayerPrefsX.GetBool(GetKey("equipped_multi", item.Id));
        private void SetEquippedMulti(MerchItem item, bool locked)
            => PlayerPrefsX.SetBool(GetKey("equipped_multi", item.Id), locked);

        [CanBeNull]
        private MerchItem EquippedSingle(MerchGroup group)
        {
            var id = PlayerPrefs.GetString(GetKey("equipped_single", group.Id));
            return id == null ? null : Find(id);
        }

        private void SetEquippedSingle(MerchItem item)
        {
            var group = GetGroup(item);
            PlayerPrefs.SetString(GetKey("equipped_single", group.Id), item.Id);
        }
    }
}