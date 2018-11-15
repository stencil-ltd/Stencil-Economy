using System;
using JetBrains.Annotations;
using Merch.UI;

namespace Merch.Data.Properties
{
    [Serializable]
    public class MerchProperties
    {
        [CanBeNull] public MerchDisplayPreset DisplayPreset;
        
        public MerchPropertyColor Color = new MerchPropertyColor();
        public MerchPropertySprite Sprite = new MerchPropertySprite();
        public MerchPropertyMaterial Material = new MerchPropertyMaterial();
        public MerchPropertyTexture Texture = new MerchPropertyTexture();
        public MerchPropertyPrefab Prefab = new MerchPropertyPrefab();        
    }
}