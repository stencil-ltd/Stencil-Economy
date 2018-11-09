using JetBrains.Annotations;
using Merch.Data;

namespace Merch.System
{
    public interface IMerchSystem
    {
        MerchResults Query(MerchQuery query);
        MerchState GetState(MerchItem item);

        [CanBeNull]
        MerchGroup GetGroup(MerchItem item);

        bool IsLocked(MerchItem item);
        void SetLocked(MerchItem item, bool locked);

        bool IsAcquired(MerchItem item);
        void SetAcquired(MerchItem item, bool acquired);

        bool IsEquipped(MerchItem item);
        void SetEquipped(MerchItem item, bool equipped);

        bool IsSelected(MerchItem item);
        void SetSelected(MerchItem item);

        void Save();
        
        // consider staging changes.
    }
}