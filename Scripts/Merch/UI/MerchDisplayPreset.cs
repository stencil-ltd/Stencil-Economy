using UnityEngine;

namespace Merch.UI
{
    [CreateAssetMenu(menuName = "Economy/Merch Display Preset")]
    public class MerchDisplayPreset : ScriptableObject
    {
        [Header("Transform")] 
        public Vector3 Position = Vector3.zero;
        public Vector3 Rotation = Vector3.zero;
        public Vector3 Scale = Vector3.one;

        [Header("UI")] 
        public bool FreezeRotation;
    }
}