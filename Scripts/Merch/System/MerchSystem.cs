using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Merch.Data;
using Plugins.Data;
using UnityEngine;
using Util;

namespace Merch.System
{
    // TODO:
    // -- Inventory system for multiples
    // -- Merge system for inventory
    
    public partial class MerchSystem : IMerchSystem
    {
        private static MerchSystem _instance;
        public static MerchSystem Instance
        {
            get
            {
                if (_instance != null) return _instance;
                _instance = new MerchSystem();
                _instance.Initialize();
                return _instance;
            }
        }
        
        private MerchGroup[] _groups;
        private Dictionary<string, MerchGroup> _groupMap;
        
        private Dictionary<string, MerchItem> _itemMap;
        private Dictionary<MerchItem, MerchGrant> _itemToGrant;
        private Dictionary<MerchItem, MerchListing> _itemToListing;
        private Dictionary<MerchItem, MerchState> _staged;
        
        private bool _skipSave;

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
            _itemMap = new Dictionary<string, MerchItem>();
            _itemToGrant = new Dictionary<MerchItem, MerchGrant>();
            _itemToListing = new Dictionary<MerchItem, MerchListing>();
            _staged = new Dictionary<MerchItem, MerchState>();
            foreach (var group in _groups)
            {
                foreach (var grant in group.Grants)
                {
                    RegisterItem(grant, group);
                    _itemToGrant[grant] = grant;
                }

                foreach (var listing in group.Listings)
                {
                    RegisterItem(listing, group);
                    if (_itemToListing.ContainsKey(listing))
                        throw new Exception($"Duplicate Item Listing! [{listing.Item}]");
                    _itemToListing[listing] = listing;
                }
            }

            if (!PlayerPrefsX.GetBool("merch_system_init"))
            {
                ApplyGrants();
                PlayerPrefsX.SetBool("merch_system_init", true);
                PlayerPrefs.Save();
            }
        }

        private void RegisterItem(MerchItem item, MerchGroup group)
        {
            item.Group = group;
            if (_itemMap.ContainsKey(item.Id))
            {
                Debug.LogError($"Duplicate Item Id! [{_itemMap[item.Id]} and {item}]");
                return;
            }
            _itemMap[item.Id] = item;
        }

        private void ApplyGrants()
        {
            _skipSave = true;
            foreach (var grant in _itemToGrant.Values)
            {
                SetLocked(grant, false);
                SetAcquired(grant, true);
            }

            var all = new MerchQuery().WithAcquired(true).Execute();
            foreach (var merchResult in all)
            {
                var group = merchResult.Item.Group;
                if (group.RequiredEquip && GetEquippedSingle(group) == null)
                    SetEquipped(merchResult.Item, true);
            }
            
            _skipSave = false;
            Save();
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
            
            return new MerchResults(query, all.ToList());
        }
    }
}