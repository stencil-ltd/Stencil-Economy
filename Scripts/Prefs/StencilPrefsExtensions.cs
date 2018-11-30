using Prefs;

namespace Scripts.Prefs
{
    public static class StencilPrefsExtensions
    {
        public static PrefHolder<T> Get<T>(this StencilPrefs prefs, string key, T defaultValue = default(T))
        {
            return PrefHolders.Get(prefs.config.Process(key), defaultValue);
        }
    }
}