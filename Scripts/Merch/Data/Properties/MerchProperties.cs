using System;

namespace Merch.Data.Properties
{
    [Serializable]
    public class MerchProperties
    {
        public MerchPropertyColor Color = new MerchPropertyColor();
        public MerchPropertySprite Sprite = new MerchPropertySprite();
        public MerchPropertyMaterial Material = new MerchPropertyMaterial();
        public MerchPropertyPrefab Prefab = new MerchPropertyPrefab();        
    }
}