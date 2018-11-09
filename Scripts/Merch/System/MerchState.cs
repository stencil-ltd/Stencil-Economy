using Common;
using Currencies;

namespace Merch.System
{
    public struct MerchState
    {
        public bool Locked;
        public bool Selected;
        public bool Acquired;
        public bool Equipped;
        
        public Price MainPrice;
        public PriceArray ExtraPrices;
    }
}