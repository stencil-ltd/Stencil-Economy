using System;
using Scripts.Maths;
using Scripts.Prefs;

namespace Scripts.Payouts
{
    public class DailyPayout
    {
        public readonly string key;
        
        public DateTime? LastPayout
        {
            get => StencilPrefs.Default.GetDateTime($"daily_payout_{key}");
            set => StencilPrefs.Default.SetDateTime($"daily_payout_{key}", value).Save();
        }

        public DailyPayout(string key)
        {
            this.key = key;
        }

        public int Peek()
        {
            var date = DateTime.Today;
            var last = LastPayout ?? DateTime.Today.AddDays(-1); // default to yesterday.
            return (date - last).Days.AtLeast(0);
        }

        public void Mark()
        {
            LastPayout = DateTime.Today;
        }

        public void Clear()
        {
            LastPayout = null;
        }
    }
}