using System;
using Merch.Data;
using Merch.System;
using Merch.UI;
using UnityEngine;
using Util;

namespace Merch.Legacy.UI
{
    public class MerchEquippedPrefab : MonoBehaviour
    {
        public MerchGroup Group;
        public string SpawnName;
        public bool ShowSelected;
        public MerchDisplayConfig Config;

        public float ScaleOnGear = 1f;

        public MerchItem Item { get; private set; }
        public GameObject Prefab => Item?.Properties.Prefab.Prefab;
        
        public GameObject Equipped { get; private set; }

        public event EventHandler OnRefresh;

        private void OnEnable()
        {
            Refresh();
            MerchSystem.Instance.OnChange += OnEquip;
        }

        private void OnDisable()
        {
            MerchSystem.Instance.OnChange -= OnEquip;
        }

        private void OnEquip(object sender, EventArgs e)
        {
            Refresh();
        }

        public void Refresh()
        {
            MerchItem buyable = null;
            var scale = 1f;
            if (ShowSelected)
                buyable = MerchSystem.Instance.Selected;
            if (buyable?.Group != Group)
            {
                buyable = MerchSystem.Instance.GetEquippedSingle(Group);
                if (buyable) scale = ScaleOnGear;
            }
            if (buyable != Item)
            {
                Debug.Log($"Equip Child: {buyable} (was {Item})");
                Item = buyable;
                transform.DestroyAllChildren();
                Equipped = Instantiate(Prefab, Vector3.zero, Quaternion.identity, transform);
                if (!string.IsNullOrEmpty(SpawnName))
                    Equipped.name = SpawnName;
            }
            transform.localScale = new Vector3(scale, scale, scale);
            Equipped.GetComponent<MerchDisplay>()?.MerchConfigure(buyable, Config);
            OnRefresh?.Invoke();
        }
    }
}