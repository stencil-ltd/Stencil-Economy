using Common;
using Malee;
using UnityEngine;

namespace Merch.Data
{
    [CreateAssetMenu(menuName = "Economy/Merch Group")]
    public class MerchGroup : StencilData
    {
        [Header("Settings")]
        public bool SingleEquip = true;
        public bool Unlockables = true;
        
        [Reorderable]
        public GrantArray Grants;
        
        [Reorderable]
        public ListingArray Listings;
    }
}