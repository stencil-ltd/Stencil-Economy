using System;
using System.Linq;
using Common;
using Dirichlet.Numerics;
using JetBrains.Annotations;
using Plugins.Data;
using Scripts.Prefs;
using UnityEngine;

#if STENCIL_ANALYTICS
using Scripts.RemoteConfig;
using Analytics;
#endif

namespace Currencies
{
    [CreateAssetMenu(menuName = "New Currency")]
    public partial class Currency : ScriptableObject, INameable
        #if STENCIL_ANALYTICS
        , IRemoteId
        #endif
    {
        public string Name;

#if STENCIL_ANALYTICS
        [RemoteField("currency_max")]
#endif
        public long Max = -1;

        #if STENCIL_ANALYTICS
        [RemoteField("currency_start")]
#endif
        public ulong StartAmount = 0;

        #if STENCIL_ANALYTICS
        [RemoteField("currency_force")]
#endif
        public long ForceAmount = -1;

        public Sprite ColorSprite;

        public Sprite PlainSprite;

        public SpriteSpecial[] SpecialSprites;

        [CanBeNull] public GameObject Sprite3D;

        public CurrencyExchange[] exchanges;

        [HideInInspector]
        public StencilPrefs Prefs = StencilPrefs.Default;

        [NonSerialized]
        private bool Silent = false;

        public string GetName() => Name;

        public string ProcessRemoteId(string id) => $"{Key.ToLower()}_{id}";

        [CanBeNull]
        public Sprite SpecialSprite(string tag)
        {
            return SpecialSprites?.FirstOrDefault(special => special.Tag == tag).Sprite;
        }

        public event EventHandler<CurrencyEvent> OnTotalChanged;
        public event EventHandler<CurrencyEvent> OnSpendableChanged;
        public event EventHandler<CurrencyEvent> OnLifetimeChanged;

        private string LegacyKey => Name?.ToLower() ?? "";
        private string Key => $"resource_{Name}";
        [NonSerialized] private bool _dirty;

        public UInt128 Total() => GetTotal();
        public UInt128 Spendable()
        {
            var total = Total();
            var staged = Staged();
            if (staged >= total) return 0;
            return total - staged;
        }
        public UInt128 Staged() => GetStaged();
        public UInt128 Lifetime() => GetLifetime();
        
        private void OnEnable()
        {
            #if STENCIL_ANALYTICS
            if (Application.isPlaying) this.BindRemoteConfig();
            #endif
            CurrencyManager.Instance.Register(this);
            InitializeData(false);
            ResetButton.OnGlobalReset += (sender, args) => Clear();
        }

        public void Clear()
        {
            InitializeData(true);
        }
        
        public UInt128? Exchange(Currency other)
        {
            return exchanges.FirstOrDefault(price => price.currency == other)?.amount;
        }

        public CurrencyOperation Add(UInt128 amount, bool raw = false) => Add(amount, false, raw);
        public CurrencyOperation Stage(UInt128 amount, bool raw = false) => Add(amount, true, raw);

        public void Unstage()
        {
            var oldTotal = Total();
            var oldSpendable = Spendable();
            SetStaged(0);
            AmountsChanged(oldTotal, oldSpendable);
        }

        private CurrencyOperation Add(UInt128 amount, bool staged, bool raw)
        {
            if (amount == 0) return Unchanged();
            if (amount < 0) return Fail();
            
            var oldTotal = Total();
            var oldSpendable = Spendable();

            var newTotal = oldTotal + amount;
            if (Max >= 0) newTotal = (UInt128) Math.Min(Max, (long) newTotal);
            SetTotal(newTotal);
            if (staged) SetStaged(GetStaged() + amount);
            MarkAdded();
            AmountsChanged(oldTotal, oldSpendable);
            if (!Silent) Debug.Log($"Add {Name} x{amount} [staged={staged}]");
            return Succeed();
        }

        private bool CanSpendInternal(UInt128 amount, out UInt128 total, out UInt128 spendable, out bool shortCircuit)
        {
            total = Total();
            spendable = Spendable();
            shortCircuit = false;
            if (amount == 0)
            {
                shortCircuit = true;
                return true;
            }
            return amount <= total;
        }

        public bool CanSpend(UInt128 amount)
        {
            return CanSpendInternal(amount, out _, out _, out _);
        }

        public CurrencyOperation Spend(UInt128 amount)
        {
            UInt128 total;
            UInt128 spendable;
            bool shortCircuit;
            if (!CanSpendInternal(amount, out total, out spendable, out shortCircuit))
                return Fail();
            if (shortCircuit)
                return Unchanged();
            var oldTotal = Total();
            var oldSpendable = Spendable();
            if (amount > spendable)
            {
                SetStaged(amount - spendable);
                amount = spendable;
            }
            SetTotal(total - amount);
            AmountsChanged(oldTotal, oldSpendable);
            if (!Silent) Debug.Log($"Spend {Name} x{amount}");
            
            #if STENCIL_ANALYTICS
            Tracking.Instance.Track($"spend_{Name}", "amount", amount);
            #endif
            return Succeed();
        }

        public CurrencyOperation Commit(UInt128 amount, bool bestEffort = false)
        {
            var oldTotal = Total();
            var oldSpendable = Spendable();
            if (amount == 0) return Unchanged();
            if (amount < 0) return Fail();
            var staged = GetStaged();
            if (amount > staged)
            {
                if (bestEffort)
                    amount = staged;
                else return Fail();
            }
            SetStaged(staged - amount);
            AmountsChanged(oldTotal, oldSpendable);
            if (!Silent) Debug.Log($"Commit {Name} x{amount}");
            return Succeed();
        }

        public void Save()
        {
            if (!_dirty) return;
            _dirty = false;
            var json = JsonUtility.ToJson(_data);
            Prefs.SetString(Key, json);
            if (!Silent) Debug.Log($"Saved {Key}:\n{json}");
        }

        private bool IsValid(long until)
        {
            return until != 0 && DateTime.FromBinary(until) > DateTime.Now;
        }
        
        private void AnythingChanged()
        {
            _dirty = true;
            if (CurrencyController.Instance != null && CurrencyController.Instance.SaveOnWrite) Save();
        }

        private void UpdateTracking()
        {
            #if STENCIL_ANALYTICS
            Tracking.Instance.SetUserProperty(Name, Total());
            Tracking.Instance.SetUserProperty($"{Name}_lifetime", Lifetime());
            #endif
        }

        private void AmountsChanged(UInt128 oldTotal, UInt128 oldSpendable)
        {
            var total = GetTotal();
            if (total > oldTotal) SetLifetime(GetLifetime() + total - oldTotal);
            if (total != oldTotal) OnTotalChanged?.Invoke(this, new CurrencyEvent(this, oldTotal, total));
            if (Spendable() != oldSpendable) OnSpendableChanged?.Invoke(this, new CurrencyEvent(this, oldSpendable, Spendable()));
            UpdateTracking();
            AnythingChanged();
        }
        
        private CurrencyOperation Fail() => CurrencyOperation.Fail(this);
        private CurrencyOperation Unchanged() => CurrencyOperation.Unchanged(this);
        private CurrencyOperation Succeed() => CurrencyOperation.Succeed(this);

        public override string ToString()
        {
            return $"{base.ToString()}, {nameof(Name)}: {Name}, {nameof(Key)}: {Key}";
        }
    }
}