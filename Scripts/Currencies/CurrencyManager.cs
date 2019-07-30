using System.Collections.Generic;
using Scripts.Prefs;
using UnityEngine;

namespace Currencies
{
    public class CurrencyManager
    {
        private static CurrencyManager _instance;
        public static CurrencyManager Instance 
            => _instance ?? (_instance = new CurrencyManager());
        
        public List<Currency> Types { get; private set; }
        private readonly Dictionary<string, Currency> _types
            = new Dictionary<string, Currency>();

        public void Register(Currency currency)
        {
            var key = currency.Name.ToLower();
            Types = Types ?? new List<Currency>();
            if (!_types.ContainsKey(key))
                Types.Add(currency);
            _types[currency.Name.ToLower()] = currency;
        }

        public Currency Get(string name)
        {
            return _types[name.ToLower()];
        }

        public Currency TryGet(string name)
        {
            _types.TryGetValue(name.ToLower(), out var retval);
            return retval;
        }
        
        public CurrencyManager Save()
        {
            foreach (var type in Types) type.Save();
            StencilPrefs.Default.Save();
            return this;
        }

    }
}