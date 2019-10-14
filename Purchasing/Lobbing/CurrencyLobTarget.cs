using System;
using System.Collections.Generic;
using Currencies;
using UnityEngine;

namespace Purchasing.Lobbing
{
    [Serializable]
    public enum CurrencyLobPrecedence
    {
        Default,
        Strong,
        Weak
    }
    
    public class CurrencyLobTarget : MonoBehaviour
    {
        private static readonly List<CurrencyLobTarget> _targets = new List<CurrencyLobTarget>();
        
        public static List<CurrencyLobTarget> GetTargets(Currency currency, LobTargetType type, bool spend)
        {
            var retval = new List<CurrencyLobTarget>();
            var weak = new List<CurrencyLobTarget>();
            var strong = new List<CurrencyLobTarget>();
            foreach (var target in _targets)
            {
                if (target.currency != currency) continue;
                var targetType = spend ? target.onSpend : target.onAcquire;
                if (type != targetType) continue;
                retval.Add(target);
                switch (target.precedence)
                {
                    case CurrencyLobPrecedence.Strong:
                        strong.Add(target);
                        break;
                    case CurrencyLobPrecedence.Weak:
                        weak.Add(target);
                        break;
                }
            }

            // If strong is present, only return strong
            if (strong.Count > 0)
                return strong;
            
            // If non-weak is present, remove all weak
            if (retval.Count > weak.Count)
                retval.RemoveAll(target => weak.Contains(target));

            return retval;
        }

        private static void AddTarget(CurrencyLobTarget target)
        {
            _targets.Add(target);
        }

        private static void RemoveTarget(CurrencyLobTarget target)
        {
            _targets.Remove(target);
        }

        public Currency currency;
        public LobTargetType onAcquire;
        public LobTargetType onSpend;
        public float delayEnd = 0f;
        public CurrencyLobPrecedence precedence = CurrencyLobPrecedence.Default;

        private void OnEnable()
        {
            AddTarget(this);
        }

        private void OnDisable()
        {
            RemoveTarget(this);
        }
    }
}