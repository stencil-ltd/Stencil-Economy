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
            => Prefs.GetBool(GetKey("locked", item.Id), true);
        private void SetLockedInternal(MerchItem item, bool locked)
            => Prefs.SetBool(GetKey("locked", item.Id), locked);
        
        private bool AcquiredInternal(MerchItem item) 
            => Prefs.GetBool(GetKey("acquired", item.Id));
        private void SetAcquiredInternal(MerchItem item, bool locked)
            => Prefs.SetBool(GetKey("acquired", item.Id), locked);
        
        private bool EquippedMulti(MerchItem item) 
            => Prefs.GetBool(GetKey("equipped_multi", item.Id));
        private void SetEquippedMulti(MerchItem item, bool locked)
            => Prefs.SetBool(GetKey("equipped_multi", item.Id), locked);

        [CanBeNull]
        private MerchItem EquippedSingle(MerchGroup group)
        {
            var id = Prefs.GetString(GetKey("equipped_single", group.Id));
            return id == null ? null : Find(id);
        }

        private void RemoveEquippedSingle(MerchGroup group)
        {
            Prefs.DeleteKey(GetKey("equipped_single", group.Id));
        }

        private void SetEquippedSingle([NotNull] MerchItem item)
        {
            var group = GetGroup(item);
            Prefs.SetString(GetKey("equipped_single", group.Id), item.Id);
        }
    }
}