using System;

namespace Merch.Data
{
    [Serializable]
    public class MerchGrant
    {
        public MerchItem Item;
        public static implicit operator MerchItem(MerchGrant grant) 
            => grant.Item;
    }
}