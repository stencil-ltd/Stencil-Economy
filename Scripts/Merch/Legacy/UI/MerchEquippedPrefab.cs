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
        public MerchDisplayConfig Config;

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
            var buyable = MerchSystem.Instance.GetEquippedSingle(Group);
            if (buyable == Item) return;
            Debug.Log($"Equip Child: {buyable} (was {Item})");
            Item = buyable;
            transform.DestroyAllChildren();
            Equipped = Instantiate(Prefab, Vector3.zero, Quaternion.identity, transform);
            Equipped.GetComponent<MerchDisplay>()?.MerchConfigure(buyable, Config);            
            if (!string.IsNullOrEmpty(SpawnName))
                Equipped.name = SpawnName;
            OnRefresh?.Invoke();
        }
    }
}