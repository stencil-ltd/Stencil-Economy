using System;
using UnityEngine;

namespace Common
{
    [Serializable]
    public abstract class StencilData : ScriptableObject
    {
        public string Id;
        public string Name;
    }
}