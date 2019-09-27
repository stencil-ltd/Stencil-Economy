using System;
using System.Collections;
using Dirichlet.Numerics;
using JetBrains.Annotations;
using Scripts.Prefs;
using Scripts.Util;
using State;
using UnityEngine;
using UnityEngine.UI;

namespace Currencies.UI
{
    public class CurrencyGenerator : MonoBehaviour
    {
        [Header("Config")]
        public Currency currency;
        public uint seconds = 3600;
        public uint amount = 1;
        public uint max;

        [Header("UI")] 
        [CanBeNull] public Text label;
        [CanBeNull] public GameObject[] others;

        private DateTime? Mark
        {
            get
            {
                var retval = StencilPrefs.Default.GetDateTime($"currency_generator_mark_{currency.Name}");
                if (retval == null) retval = Mark = DateTime.UtcNow;
                return retval.Value;
            }
            set => StencilPrefs.Default.SetDateTime($"currency_generator_mark_{currency.Name}", value).Save();
        }

        public TimeSpan? TimeRemaining()
        {
            if (IsMaxed()) return null;
            return NextUtc() - DateTime.UtcNow;
        }

        public DateTime? NextUtc()
        {
            if (IsMaxed()) return null;
            return Mark?.AddSeconds(seconds);
        }

        private void OnEnable()
        {
            StartCoroutine(_Tick());
        }

        private void Update()
        {
            if (label == null && others == null) return;
            var remaining = TimeRemaining();
            label.text = remaining?.ToString(@"hh\:mm\:ss") ?? "";
            if (others != null)
                foreach (var other in others)
                    other.gameObject.SetActive(remaining != null);
        }

        private IEnumerator _Tick()
        {
            while (true)
            {
                if (IsMaxed())
                {
                    Mark = null;
                    yield break;
                }
                while (TimeRemaining() <= TimeSpan.Zero)
                {
                    Mark = NextUtc();
                    currency.Add(amount).AndSave();
                }
                yield return new WaitForSeconds(1);
            }
        }

        private bool IsMaxed()
        {
            return max > 0 && currency.Total() >= max;
        }
    }
}