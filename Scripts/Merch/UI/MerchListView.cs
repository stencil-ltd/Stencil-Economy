using System;
using Merch.Data;
using Merch.System;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace Merch.UI
{
    public class MerchListView : MonoBehaviour
    {
        public MerchGroup Group;
        public MerchItemView ItemViewPrefab;
        public LayoutGroup Content;

        private MerchQuery _query;
        private MerchResults _results;

        private void OnEnable()
        {
            AddListener();
            Populate();
        }

        private void OnDisable()
        {
            RemoveListener();
        }

        private void SetGroup(MerchGroup group)
        {
            if (Group == group) return;
            RemoveListener();
            Group = group;
            AddListener();
            Populate();
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