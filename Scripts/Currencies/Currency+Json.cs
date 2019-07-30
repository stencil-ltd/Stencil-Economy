using System;
using System.Collections.Generic;
using Dirichlet.Numerics;
using UnityEngine;

namespace Currencies
{
    public partial class Currency
    {
        [NonSerialized]
        private CurrencyModifier _data;

        private void InitializeData(bool reset)
        {
            var str = Prefs.GetString(Key);
            var fresh = reset || string.IsNullOrEmpty(str);
            if (!fresh)
                _data = JsonUtility.FromJson<CurrencyModifier>(str);
            else
                _data = new CurrencyModifier
                {
                    Total = StartAmount,
                    Lifetime = StartAmount
                };
            
            var force = ForceAmount;
            if (Prefs.HasKey(LegacyKey))
            {
                force = Prefs.GetInt(LegacyKey);
                Prefs.DeleteKey(LegacyKey);
            }
            if (force >= 0) _data.Total = (ulong) force;
            _dirty = fresh || force >= 0;
            UpdateTracking();
        }

        private void MarkAdded() => _data.HasAdded = true;
        public bool HasAdded() => _data.HasAdded;

        private void SetTotal(UInt128 total) => _data.Total = total;
        private UInt128 GetTotal() => _data.Total;

        private void SetStaged(UInt128 staged) => _data.Staged = staged;
        private UInt128 GetStaged() => _data.Staged;

        private void SetLifetime(UInt128 lifetime)
        {
            var old = _data.Lifetime;
            _data.Lifetime = lifetime;
            OnLifetimeChanged?.Invoke(this, new CurrencyEvent(this, old, lifetime));
        }

        private UInt128 GetLifetime() => _data.Lifetime;
    }
}