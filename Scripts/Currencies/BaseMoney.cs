using System;
using System.Collections.Generic;
using Analytics;
using Binding;
using Scripts.Prefs;
using State;
using UnityEngine;

namespace Currencies
{
    public abstract partial class BaseMoney<T> : StencilData, IMoney<T> where T : IFormattable, IComparable, IComparable<T>, IEquatable<T>
    {
        #region Fields

        public Sprite sprite;
        
        [RemoteField("currency_start")]
        public ulong start;
        
        [NonSerialized] 
        public StencilPrefs prefs = StencilPrefs.Default;

        #endregion

        #region Properties
        
        protected string Key => $"resource_{Id}";
        [NonSerialized] protected bool _dirty;

        #endregion

        #region Private Fields
        
        private T _total;
        private T _staged;
        private T _lifetime;

        #endregion
        
        #region Events
        
        #endregion

        #region Public
        
        protected MoneyOperation<T> Fail() => MoneyOperation<T>.Fail(this);
        protected MoneyOperation<T> Unchanged() => MoneyOperation<T>.Unchanged(this);
        protected MoneyOperation<T> Succeed() => MoneyOperation<T>.Succeed(this);

        public event EventHandler<IMoney<T>> OnTotalChanged;
        protected void NotifyTotalChanged() => OnTotalChanged?.Invoke(this, this);
        
        public event EventHandler<IMoney<T>> OnSpendableChanged;
        protected void NotifySpendableChanged() => OnSpendableChanged?.Invoke(this, this);
        
        public string GetName() => Name;
        public string ProcessRemoteId(string id) => $"{Key.ToLower()}_{id}";

        #endregion
        
        #region Protected

        protected abstract void OnChanged(T oldTotal, T oldSpendable);
        protected abstract void Deserialize(string json);
        protected abstract string Serialize();
        
        protected void UpdateTracking()
        {
            Tracking.Instance.SetUserProperty(Name, Total());
        }
        
        #endregion
        
        #region Private
        
        private void InitializeData(bool reset)
        {
            var str = prefs.GetString(Key);
            var fresh = reset || string.IsNullOrEmpty(str);
            if (!fresh)
            {
                Deserialize(str);
            } else
            {
                _total = (T)(object) start;
                _lifetime = _total;
                _staged = default(T);
            }
            _dirty = fresh;
            UpdateTracking();
        }
        
        #endregion
    }
}