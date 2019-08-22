#if UNITY_PURCHASING
using UnityEngine.Purchasing;

namespace Stencil.Economy.Purchasing
{
    public interface IStencilProductState
    {
        Product product { get; }
        void TrackPurchase();
        void CheckSubscription();
    }
}

#endif