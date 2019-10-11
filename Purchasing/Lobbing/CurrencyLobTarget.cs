using System.Collections.Generic;
using System.Linq;
using Currencies;
using UnityEngine;

namespace Purchasing.Lobbing
{
    public class CurrencyLobTarget : MonoBehaviour
    {
        private static readonly List<CurrencyLobTarget> _targets = new List<CurrencyLobTarget>();
        
        public static List<CurrencyLobTarget> GetTargets(Currency currency, LobTargetType type, bool spend)
        {
            var retval = new List<CurrencyLobTarget>();
            foreach (var target in _targets)
            {
                if (target.currency != currency) continue;
                var targetType = spend ? target.onSpend : target.onAcquire;
                if (type != targetType) continue;
                retval.Add(target);
            }
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