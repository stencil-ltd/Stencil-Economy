using System.Collections.Generic;

namespace Merch.System
{
    public class MerchResults
    {
        public readonly MerchQuery Query;
        public readonly IReadOnlyList<MerchResult> Results;
        public MerchResults(MerchQuery query, List<MerchResult> results)
        {
            Query = query;
            Results = results;
        }

        public static bool RoughlyEqual(MerchResults a, MerchResults b)
        {
            if (a == null && b == null) return true;
            if (a == null || b == null) return false;
            if (a.Results.Count != b.Results.Count) return false;
            for (var i = 0; i < a.Results.Count; i++)
                if (!MerchResult.RoughlyEqual(a.Results[i], b.Results[i]))
                    return false;
            return true;
        }
    }
}