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
        
        public DateTime? LastPayout
        {
            get => StencilPrefs.Default.GetDateTime($"daily_payout_{key}");
            set => StencilPrefs.Default.SetDateTime($"daily_payout_{key}", value).Save();
        }

        public DailyPayout(string key)
        {
            this.key = key;
            prod = StencilRemote.IsProd();
            interval = prod ? TimeSpan.FromDays(1) : TimeSpan.FromMinutes(5);
        }

        public int Peek()
        {
            var date = Now();
            var last = LastPayout ?? date - interval; // default to yesterday.
            return (int) ((date - last).Ticks / interval.Ticks).AtLeast(0);
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
            return prod ? DateTime.Today : DateTime.Now;
        }
    }
}