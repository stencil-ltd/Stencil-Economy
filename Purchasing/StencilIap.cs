using System;
using UnityEngine.Purchasing;

namespace Scripts.Purchasing
{
    public static class StencilIap
    {
        public static void ApplyPlatformHacks(this IAPButton button)
        {
            ApplyAndroidHack(button);
            ApplyEditorHack(button);
        }
        
        private static void ApplyAndroidHack(this IAPButton button)
        {
#if UNITY_ANDROID
            var str = button.titleText?.text;
            if (str?.Contains("(") == true)
                button.titleText.text = str.Substring(0, str.IndexOf("(", StringComparison.InvariantCulture) - 1);
#endif
        }

        private static void ApplyEditorHack(this IAPButton button)
        {
#if UNITY_EDITOR
            button.priceText.text = "$" + button.priceText.text;
#endif
        }
    }
}