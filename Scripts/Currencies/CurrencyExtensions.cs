using UnityEngine;

namespace Currencies
{
    public static class CurrencyExtensions
    {
        public static Sprite BestSprite(this Currency res)
        {
            Sprite icon = null;
            if (icon == null) icon = res.SpecialSprite("small");
            if (icon == null) icon = res.ColorSprite;
            if (icon == null) icon = res.PlainSprite;
            return icon;
        }
    }
}