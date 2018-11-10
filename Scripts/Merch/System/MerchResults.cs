using System.Collections.Generic;
using System.Linq;

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
            if (results.Count > 0 && query.Autoselect && !results.Any(result => result.State.Selected))
            {
                var first = results.First();
                first.Autoselect();
            }
        }
    }
}