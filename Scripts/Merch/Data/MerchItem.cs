using Common;
using Merch.Data.Properties;
using UnityEngine;

namespace Merch.Data
{
    [CreateAssetMenu(menuName = "Economy/Merch Item")]
    public class MerchItem : StencilData
    {
        public MerchProperties Properties = new MerchProperties();
    }
}