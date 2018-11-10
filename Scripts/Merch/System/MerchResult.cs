using Merch.Data;

namespace Merch.System
{
    public struct MerchResult
    {
        public readonly MerchItem Item;
        public MerchState State;

        public MerchResult(MerchItem item, MerchState state)
        {
            Item = item;
            State = state;
        }

        public MerchResult(IMerchSystem system, MerchGroup group, MerchGrant grant)
        {
            Item = grant.Item;
            State = new MerchState();
            GetState(system, Item, ref State);
        }

        public MerchResult(IMerchSystem system, MerchGroup group, MerchListing listing)
        {
            Item = listing.Item;
            State = new MerchState();
            GetState(system, Item, ref State);
            State.MainPrice = listing.MainPrice;
            State.ExtraPrices = listing.ExtraPrices;
        }

        public void Autoselect()
        {
            var state = State;
            state.Selected = true;
            State = state;
        }

        private static void GetState(IMerchSystem system, MerchItem item, ref MerchState state)
        {
            state.Locked = system.IsLocked(item);
            state.Selected = system.IsSelected(item);
            state.Acquired = system.IsAcquired(item);
            state.Equipped = system.IsEquipped(item);
        }
    }
}