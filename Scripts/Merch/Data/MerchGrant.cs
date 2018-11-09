using System;

namespace Merch.Data
{
    [Serializable]
    public class MerchGrant
    {
        public bool Unlocked = true;
        public bool Acquired = true;
        public MerchItem Item;

        public static implicit operator MerchItem(MerchGrant grant) 
            => grant.Item;
    }
}