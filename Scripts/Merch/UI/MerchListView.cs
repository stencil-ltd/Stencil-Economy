using System;
using System.Linq;
using Merch.Data;
using Merch.System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Util;

namespace Merch.UI
{
    public class MerchListView : MonoBehaviour
    {
        [Serializable]
        public class MerchListEvent : UnityEvent<MerchGroup>
        {}
        
        public MerchGroup Group;
        public MerchItemView ItemViewPrefab;
        public LayoutGroup Content;
        public bool ClearSelectedOnExit = true;

        public MerchListEvent OnSetGroup;

        private MerchQuery _query;
        private MerchResults _results;

        private void OnEnable()
        {
            SetGroup(Group);
        }

        private void OnDisable()
        {
            RemoveListener();
            if (ClearSelectedOnExit)
                MerchSystem.Instance.SetSelected(null);
        }

        public void SetGroup(MerchGroup group)
        {
            Debug.Log($"Set group to {group}");
            RemoveListener();
            Group = group;
            var selected = MerchSystem.Instance.GetEquippedSingle(group);
            if (selected?.Group != group)
                selected = MerchSystem.Instance.GetItems(group).FirstOrDefault();
            MerchSystem.Instance.SetSelected(selected);
            Refresh();
        }

        private void Refresh()
        {
            AddListener();
            Populate();
            OnSetGroup?.Invoke(Group);
        }

        private void AddListener()
        {
            if (Group != null)
                Group.OnChange += OnChange;
        }

        private void RemoveListener()
        {
            if (Group != null)
                Group.OnChange -= OnChange;
        }

        private void Populate()
        {
            if (Group == null) return;
            _query = Group.Query().WithLocked(false);
            var old = _results;
            _results = _query.Execute();
            var same = MerchResults.RoughlyEqual(old, _results);
            if (!same) Content.transform.DestroyAllChildren();
            for (var i = 0; i < _results.Results.Count; ++i)
            {
                var result = _results.Results[i];
                MerchItemView listing;
                if (!same) 
                    listing = Instantiate(ItemViewPrefab, Content.transform);
                else 
                    listing = Content.transform.GetChild(i).GetComponent<MerchItemView>();
                listing.SetResult(result);
            }
        }

        private void OnChange(object sender, EventArgs e)
        {
            Populate();
        }
    }
}