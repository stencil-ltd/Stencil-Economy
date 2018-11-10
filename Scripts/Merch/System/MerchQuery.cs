using System.Collections.Generic;
using System.Linq;
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

        public bool Autoselect;

        public MerchResults Execute([CanBeNull] IMerchSystem system = null)
        {
            system = system ?? MerchSystem.Instance;
            return system.Query(this);
        }

        public MerchQuery WithGroup(MerchGroup group)
        {
            Group = group;
            return this;
        }

        public MerchQuery WithCurrencies(params Currency[] currencies)
        {
            AcceptedCurrencies = currencies?.ToList();
            return this;
        }

        public MerchQuery WithLocked(bool? value)
        {
            Locked = value;
            return this;
        }

        public MerchQuery WithAcquired(bool? value)
        {
            Acquired = value;
            return this;
        }

        public MerchQuery WithEquipped(bool? value)
        {
            Equipped = value;
            return this;
        }

        public MerchQuery WithSelected(bool? value)
        {
            Selected = value;
            return this;
        }

        public MerchQuery WithAutoselect(bool value)
        {
            Autoselect = value;
            return this;
        }
    }
}