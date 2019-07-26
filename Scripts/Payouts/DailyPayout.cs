using System;
using Scripts.Maths;
using Scripts.Prefs;
using Scripts.RemoteConfig;

namespace Scripts.Payouts
{
    public class DailyPayout
    {
        public readonly string key;
        public readonly TimeSpan interval;
        public readonly bool prod;
        public readonly bool shorten;

        public int maxMult = 7;
        
        public DateTime? LastPayout
        {
            get => StencilPrefs.Default.GetDateTime($"daily_payout_{key}");
            set => StencilPrefs.Default.SetDateTime($"daily_payout_{key}", value).Save();
        }

        public DailyPayout(string key)
        {
            this.key = key;
            prod = StencilRemote.IsProd();
            shorten = !prod;
            #if !UNITY_ANDROID
            shorten = false;
            #endif
            interval = !shorten ? TimeSpan.FromDays(1) : TimeSpan.FromMinutes(5);
        }

        public int Peek()
        {
            var date = Now();
            var last = LastPayout ?? date - interval; // default to yesterday.
            var mult = (int) ((date - last).Ticks / interval.Ticks).AtLeast(0).AtMost(maxMult);
            return mult;
        }

        public void Mark()
        {
            LastPayout = Now();
        }

        public void Clear()
        {
            LastPayout = null;
        }

        private DateTime Now()
        {
            return !shorten ? DateTime.Today : DateTime.Now;
        }
    }
}