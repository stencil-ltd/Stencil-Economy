using System;
using Scripts.Maths;
using Scripts.Prefs;
using UnityEngine;

namespace Scripts.Payouts
{
    public class DailyPayout
    {
        public readonly string key;
        public TimeSpan interval;
        public int maxMult = 7;
        
        // If both, it will take the most generous payout interpretation.
        public bool useDayBoundary = true; 
        public bool useIntervalBoundary = true;
        
        public DateTime? LastPayout
        {
            get => StencilPrefs.Default.GetDateTime($"daily_payout_{key}");
            set => StencilPrefs.Default.SetDateTime($"daily_payout_{key}", value).Save();
        }

        public DailyPayout(string key)
        {
            this.key = key;
            interval = TimeSpan.FromDays(1);
        }

        public int Peek()
        {
            var date = Now();
            var last = LastPayout ?? (date - interval); // default to yesterday.
            var diff = date - last;
            
            // we're going to either take the raw number of intervals that have passed (i.e. whole spans of 24h)...
            var tickmult = useIntervalBoundary ? (int) (diff.Ticks / interval.Ticks) : 0;
            
            // or we're going to take the number of calendar days that have passed. 
            var daymult = useDayBoundary ? (int) diff.TotalDays : 0;
            
            // Whichever is most generous.
            var mult = Math.Max(tickmult, daymult).AtLeast(0).AtMost(maxMult);

            Debug.Log($"DailyPayout: {tickmult} vs {daymult} = {mult}. Last payout was {last}");
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