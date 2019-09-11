using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Purchasing;

namespace Scripts.Purchasing
{
    public static class StencilIap
    {
        public static void ApplyPlatformHacks(this IAPButton button, bool update = true)
        {
            if (update)
            {
                var method = typeof(IAPButton).GetMethod("UpdateText",BindingFlags.NonPublic | BindingFlags.Instance);
                if (method == null)
                {
                    Debug.LogError("Missing UpdateText Method");
                }
                else
                {
                    method.Invoke(button, null);
                }
            }
            ApplyAndroidHack(button);
            ApplyEditorHack(button);
        }

        private static void ApplyAndroidHack(this IAPButton button)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            var str = button.titleText?.text;
            if (str?.Contains("(") == true)
                button.titleText.text = str.Substring(0, str.IndexOf("(", StringComparison.InvariantCulture) - 1);
#endif
        }

        private static void ApplyEditorHack(this IAPButton button)
        {
#if UNITY_EDITOR
            if (button.priceText != null && !button.priceText.text.StartsWith("$"))
                button.priceText.text = "$" + button.priceText.text;
#endif
        }
    }
}