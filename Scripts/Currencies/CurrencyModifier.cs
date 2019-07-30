using System;
using System.Collections.Generic;
using Dirichlet.Numerics;

namespace Currencies
{
    [Serializable]
    internal class CurrencyModifier
    {
        public bool HasAdded;
        public UInt128 Total;
        [NonSerialized] public UInt128 Staged;

        public UInt128 Lifetime
        {
            set => lifetime = value.ToString();
            get
            {
                UInt128.TryParse(lifetime, out var retval);
                return retval;
            }
        }

        public string lifetime;
    }
}