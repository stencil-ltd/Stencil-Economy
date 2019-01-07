using System;
using Dirichlet.Numerics;
using UnityEngine;

namespace Currencies
{
    public abstract partial class BaseMoney<T>
    {
        #region Concrete

        public Sprite Sprite() => sprite;

        public T Total() => _total;
        public T Staged() => _staged;
        public T Lifetime() => _lifetime;
        
        protected void SetTotal(T value) => _total = value;
        protected void SetStaged(T value) => _staged = value;
        protected void SetLifetime(T value) => _lifetime = value;
        
        public bool CanSpend(T amount) => Spendable().CompareTo(amount) > 0;
        public void Clear() => InitializeData(true);
        
        public void Save()
        {
            if (!_dirty) return;
            _dirty = false;
            var json = Serialize();
            prefs.SetString(Key, json);
            Debug.Log($"Saved {Key}:\n{json}");
        }

        #endregion

        public abstract T Spendable();

        public abstract MoneyOperation<T> Add(T amount, bool staged = false);

        public abstract MoneyOperation<T> Spend(T amount, bool staged = false);

        public abstract MoneyOperation<T> Commit(T amount);
    }
}