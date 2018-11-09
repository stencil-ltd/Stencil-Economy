using System;
using UnityEngine;

namespace Common
{
    [Serializable]
    public abstract class StencilData : ScriptableObject
    {
        public string Id;
        public string Name;

        public override string ToString()
        {
            return $"{base.ToString()}, {nameof(Id)}: {Id}, {nameof(Name)}: {Name}";
        }
    }
}