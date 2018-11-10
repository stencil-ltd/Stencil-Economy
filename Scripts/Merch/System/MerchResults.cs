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
    }
}