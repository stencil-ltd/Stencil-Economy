using System;
using UnityEngine;
using Util;

namespace Scripts.Prefs
{
    [Serializable]
    public class StencilPrefs
    {
        public static StencilPrefs Default = new StencilPrefs();
        
        public PrefConfig config;
        public StencilPrefs(PrefConfig config)
        {
            this.config = config;
        }

        public StencilPrefs()
        {}

        #region PlayerPrefs

        public int GetInt(string key, int defaultValue) => PlayerPrefs.GetInt(config.Process(key), defaultValue);
        public StencilPrefs SetInt(string key, int value)
        {
            PlayerPrefs.SetInt(config.Process(key), value);
            return this;
        }
        
        public long GetLong(string key, long defaultValue) => PlayerPrefsX.GetLong(config.Process(key), defaultValue);
        public StencilPrefs SetLong(string key, long value)
        {
            PlayerPrefsX.SetLong(config.Process(key), value);
            return this;
        }
        
        public string GetString(string key, string defaultValue) => PlayerPrefs.GetString(config.Process(key), defaultValue);
        public StencilPrefs SetString(string key, string value)
        {
            PlayerPrefs.SetString(config.Process(key), value);
            return this;
        }
        
        public float GetFloat(string key, float defaultValue) => PlayerPrefs.GetFloat(config.Process(key), defaultValue);
        public StencilPrefs SetFloat(string key, float value)
        {
            PlayerPrefs.SetFloat(config.Process(key), value);
            return this;
        }
        
        public bool GetBool(string key, bool defaultValue) => PlayerPrefsX.GetBool(config.Process(key), defaultValue);
        public StencilPrefs SetBool(string key, bool value)
        {
            PlayerPrefsX.SetBool(config.Process(key), value);
            return this;
        }
        
        public DateTime? GetDateTime(string key, DateTime? defaultValue) => PlayerPrefsX.GetDateTime(config.Process(key), defaultValue);
        public StencilPrefs SetDateTime(string key, DateTime? value)
        {
            PlayerPrefsX.SetDateTime(config.Process(key), value);
            return this;
        }
        
        #endregion
    }
}