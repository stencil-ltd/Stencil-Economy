using System;

namespace Merch.Data.Properties
{
    [Serializable]
    public abstract class MerchProperty : IMerchProperty
    {
        public bool Enabled;
        public bool IsEnabled => Enabled;
    }
}