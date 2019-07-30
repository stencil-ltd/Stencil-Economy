using System;
using System.Collections.Generic;
using Plugins.UI;
using UI;
using UnityEngine;
using Util;

namespace Currencies
{
    [ExecuteInEditMode]
    public class CurrencyController : Permanent<CurrencyController>
    {
        public bool SaveOnWrite;
        
        [Obsolete]
        public Currency[] Types = { };

        [Obsolete]
        public Currency Get(string name)
        {
            return CurrencyManager.Instance.Get(name);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Save();
        }

        public CurrencyController Save()
        {
            if (!Application.isPlaying) return this;
            foreach (var type in CurrencyManager.Instance.Types)
                type?.Save();
            PlayerPrefs.Save();
            return this;
        }

        public CurrencyController Clear()
        {
            if (!Application.isPlaying) return this;
            foreach (var type in CurrencyManager.Instance.Types)
                type.Clear();
            return this;
        }
    }
}