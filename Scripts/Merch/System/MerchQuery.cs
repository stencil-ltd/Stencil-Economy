using System.Collections.Generic;
using Currencies;
using JetBrains.Annotations;
using Merch.Data;

namespace Merch.System
{
    public class MerchQuery
    {
        [CanBeNull] public MerchGroup Group;
        [CanBeNull] public List<Currency> AcceptedCurrencies;
        
        public bool? Locked;
        public bool? Acquired;
        public bool? Equipped;
        public bool? Selected;

        public MerchResults Execute(IMerchSystem system) 
            => system.Query(this);
    }
}