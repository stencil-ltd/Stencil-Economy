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

        public readonly int maxMult;
        
        public DateTime? LastPayout
        {
            get => StencilPrefs.Default.GetDateTime($"daily_payout_{key}");
            set => StencilPrefs.Default.SetDateTime($"daily_payout_{key}", value).Save();
        }

        public DailyPayout(string key, int maxMult = 7)
        {
            this.key = key;
            this.maxMult = maxMult;
            interval = TimeSpan.FromDays(1);
        }

        public int Peek()
        {
            var date = Now();
            var last = LastPayout ?? date - interval; // default to yesterday.
            var diff = date - last;
            
            // we're going to either take the raw number of intervals that have passed (i.e. whole spans of 24h)...
            var tickmult = (int) (diff.Ticks / interval.Ticks);
            
            // or we're going to take the number of calendar days that have passed. 
            var daymult = (int) diff.TotalDays;
            
            // Whichever is most generous.
            var mult = Math.Max(tickmult, daymult).AtLeast(0).AtMost(maxMult);
            
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
            return DateTime.Now;
        }
    }
}