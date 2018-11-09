using Merch.Data;
using Util;
// ReSharper disable SuggestBaseTypeForParameter

namespace Merch.System
{
    public partial class MerchSystem
    {
        private static string GetKey(string name, string id)
            => $"merch_{name}_{id}";
        
        private static bool LockedInternal(MerchItem item) 
            => PlayerPrefsX.GetBool(GetKey("locked", item.Id), true);
        private static void SetLockedInternal(MerchItem item, bool locked)
            => PlayerPrefsX.SetBool(GetKey("locked", item.Id), locked);
        
        private static bool AcquiredInternal(MerchItem item) 
            => PlayerPrefsX.GetBool(GetKey("acquired", item.Id));
        private static void SetAcquiredInternal(MerchItem item, bool locked)
            => PlayerPrefsX.SetBool(GetKey("acquired", item.Id), locked);
        
        private static bool EquippedInternal(MerchItem item) 
            => PlayerPrefsX.GetBool(GetKey("equipped", item.Id));
        private static void SetEquippedInternal(MerchItem item, bool locked)
            => PlayerPrefsX.SetBool(GetKey("equipped", item.Id), locked);
    }
}