using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Merch.Data;
using Plugins.Data;
using UnityEngine;

namespace Merch.System
{
    public partial class MerchSystem : IMerchSystem
    {
        private MerchGroup[] _groups;
        private Dictionary<string, MerchGroup> _groupMap;
        private Dictionary<MerchItem, MerchGroup> _itemToGroup;
        private Dictionary<MerchItem, MerchGrant> _itemToGrant;
        private Dictionary<MerchItem, List<MerchListing>> _itemToListings;

        [CanBeNull] public MerchItem Selected { get; private set; }

        public event EventHandler OnChange;

        public MerchSystem()
        {
            ResetButton.OnGlobalReset += (sender, args) =>
            {
                Initialize();
                Notify();
            };
        }

        public void Initialize()
        {
            _groups = Resources.FindObjectsOfTypeAll<MerchGroup>().ToArray();
            _groupMap = _groups.ToDictionary(group => group.Id);
            _itemToGroup = new Dictionary<MerchItem, MerchGroup>();
            foreach (var group in _groups)
            {
                foreach (var grant in group.Grants)
                    _itemToGroup[grant] = group;
                foreach (var listing in group.Listings)
                    _itemToGroup[listing] = group;
            }
            _itemToGrant = new Dictionary<MerchItem, MerchGrant>();
            foreach (var group in _groups)
                foreach (var grant in group.Grants)
                    _itemToGrant[grant] = grant;
            _itemToListings = new Dictionary<MerchItem, List<MerchListing>>();
            foreach (var group in _groups)
                foreach (var listing in group.Listings)
                {
                    if (!_itemToListings.ContainsKey(listing))
                        _itemToListings[listing] = new List<MerchListing>();
                    _itemToListings[listing].Add(listing);
                }
        }

        public MerchResults Query(MerchQuery query)
        {
            var groups = _groups;
            if (query.Group != null)
                groups = new[] {query.Group};

            var all = groups.SelectMany(group =>
            {
                var grants = group.Grants.Select(grant => new MerchResult(this, group, grant));
                var listings = group.Listings.Select(listing => new MerchResult(this, group, listing));
                return grants.Concat(listings);
            });

            if (query.AcceptedCurrencies != null)
                all = all.Where(result => result.QueryCurrencies(query.AcceptedCurrencies));
 
            if (query.Locked != null)
                all = all.Where(result => result.State.Locked == query.Locked);
            
            if (query.Acquired != null)
                all = all.Where(result => result.State.Acquired == query.Acquired);
            
            if (query.Equipped != null)
                all = all.Where(result => result.State.Equipped == query.Equipped);
            
            if (query.Selected != null)
                all = all.Where(result => result.State.Selected == query.Selected);
            
            return new MerchResults(all.ToList());
        }
    }
}