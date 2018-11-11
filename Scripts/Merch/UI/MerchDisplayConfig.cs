using System;

namespace Merch.UI
{
    [Serializable]
    public struct MerchDisplayConfig
    {
        [Serializable]
        public enum DisplayMode
        {
            Storefront,
            Play
        }

        public DisplayMode Mode;
        public bool ShowGear;
        public bool ResetScale;
    }
}