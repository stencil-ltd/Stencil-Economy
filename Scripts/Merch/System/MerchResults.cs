using System.Collections.Generic;
using Merch.Data;

namespace Merch.System
{
    public class MerchResults
    {
        public readonly IReadOnlyList<MerchResult> Results;
        public MerchResults(List<MerchResult> results)
        {
            Results = results;
        }
    }
}