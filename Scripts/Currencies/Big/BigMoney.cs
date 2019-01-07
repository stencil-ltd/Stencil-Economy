using System;
using Dirichlet.Numerics;
using UnityEngine;

namespace Currencies.Big
{
    [CreateAssetMenu(menuName = "Stencil/Big Money")]
    public partial class BigMoney : BaseMoney<UInt128>
    {
        [Serializable]
        private struct Data
        {
            public string total;
            public string lifetime;
        }

        protected override void OnChanged(UInt128 oldTotal, UInt128 oldSpendable)
        {
            var total = Total();
            if (total > oldTotal) SetLifetime(Lifetime() + total - oldTotal);
            if (total != oldTotal) NotifyTotalChanged();
            if (Spendable() != oldSpendable) NotifySpendableChanged();
            UpdateTracking();
            _dirty = true;
        }

        protected override string Serialize()
        {
            var data = new Data {total = Total().ToString(), lifetime = Lifetime().ToString()};
            return JsonUtility.ToJson(data);
        }

        protected override void Deserialize(string json)
        {
            var data = JsonUtility.FromJson<Data>(json);
            SetTotal(UInt128.Parse(data.total));
            SetLifetime(UInt128.Parse(data.lifetime));
        }
    }
}