using System.Collections.Generic;
using Currencies;
using JetBrains.Annotations;
using Merch.Data;

namespace Merch.System
{
    public interface IMerchSystem
    {
        MerchResults Query(MerchQuery query);
        MerchState GetState(MerchItem item);

        [CanBeNull] 
        MerchItem Find(string id);

        [CanBeNull]
        MerchGroup GetGroup(MerchItem item);

        IEnumerable<MerchItem> GetItems(MerchGroup group);

        bool IsLocked(MerchItem item);
        void SetLocked(MerchItem item, bool locked);

        bool IsAcquired(MerchItem item);
        void SetAcquired(MerchItem item, bool acquired);

        bool IsEquipped(MerchItem item);
        void SetEquipped(MerchItem item, bool equipped);
        [CanBeNull] MerchItem GetEquippedSingle(MerchGroup group);

        bool IsSelected(MerchItem item);
        void SetSelected(MerchItem item);

        bool CanPurchase(MerchItem item, Currency currency);
        bool AttemptPurchase(MerchItem item, Currency currency);

        void Save();
        
        // consider staging changes.
    }
}