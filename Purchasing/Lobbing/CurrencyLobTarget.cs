using System;
using System.Collections.Generic;
using System.Linq;
using Currencies;
using Plugins.Collections;
using UnityEngine;

namespace Purchasing.Lobbing
{
    public class CurrencyLobTarget : MonoBehaviour
    {
        private static readonly Dictionary<Currency, List<CurrencyLobTarget>> _to 
            = new Dictionary<Currency, List<CurrencyLobTarget>>();
        private static readonly Dictionary<Currency, List<CurrencyLobTarget>> _from 
            = new Dictionary<Currency, List<CurrencyLobTarget>>();

        public static List<CurrencyLobTarget> GetTargets(Currency currency, LobTargetType type)
        {
            var dict = type == LobTargetType.To ? _to : _from;
            return new List<CurrencyLobTarget>(dict.GetValueOrDefault(currency));
        }

        private static void AddTarget(CurrencyLobTarget target)
        {
            var dict = target.type == LobTargetType.To ? _to : _from;
            if (!dict.ContainsKey(target.currency)) 
                dict[target.currency] = new List<CurrencyLobTarget>();
            dict[target.currency].Add(target);
        }

        private static void RemoveTarget(CurrencyLobTarget target)
        {
            var dict = target.type == LobTargetType.To ? _to : _from;
            if (!dict.ContainsKey(target.currency)) return;
            dict.Remove(target.currency);
        }

        public Currency currency;
        public LobTargetType type;

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