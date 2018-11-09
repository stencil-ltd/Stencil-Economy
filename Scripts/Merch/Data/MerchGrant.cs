using System;

namespace Merch.Data
{
    [Serializable]
    public class MerchGrant
    {
        [Serializable]
        public enum GrantType
        {
            Unlock,
            Acquire,
            Equip
        }

        public GrantType Type = GrantType.Equip;
        public MerchItem Item;

        public static implicit operator MerchItem(MerchGrant grant) 
            => grant.Item;
    }
}