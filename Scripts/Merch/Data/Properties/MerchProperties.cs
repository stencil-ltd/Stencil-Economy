using System;
using JetBrains.Annotations;
using Merch.UI;
using UnityEngine;

namespace Merch.Data.Properties
{
    [Serializable]
    public class MerchProperties
    {
        [CanBeNull] public MerchDisplayPreset DisplayPreset;

        public bool HasColor;
        public Color Color;
        public Sprite Sprite;
        public Material Material;
        public Texture Texture;
        public GameObject Prefab;        
    }
}