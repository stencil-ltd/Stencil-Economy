using System;
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

        public MerchListEvent OnSetGroup;

        private MerchQuery _query;
        private MerchResults _results;

        private void OnEnable()
        {
            Refresh();
        }

        private void OnDisable()
        {
            RemoveListener();
        }

        public void SetGroup(MerchGroup group)
        {
            if (Group == group) return;
            RemoveListener();
            Group = group;
            Refresh();
        }

        private void Refresh()
        {
            AddListener();
            MerchSystem.Instance.SetSelected(null);
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
            Content.transform.DestroyAllChildren();
            _query = Group.Query().WithLocked(false);
            _results = _query.Execute();
            foreach (var result in _results.Results)
            {
                var listing = Instantiate(ItemViewPrefab, Content.transform);
                listing.SetResult(result);
            }
        }

        private void OnChange(object sender, EventArgs e)
        {
            Populate();
        }
    }
}