using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Common
{
    [Serializable]
    public abstract class StencilData : ScriptableObject, INameable, IIdentifiable
    {
        private static Dictionary<string, StencilData> _idMap 
            = new Dictionary<string, StencilData>();
        
        public string Id;
        public string Name;

        public string GetId() => Id;
        public string GetName() => Name;

        public static void Reload()
        {
            var old = _idMap;
            _idMap = new Dictionary<string, StencilData>();
            foreach (var stencilData in old.Values)
                _idMap[stencilData.Id] = stencilData;
        }
        
        [CanBeNull]
        public static StencilData Find(string id)
        {
            if (!_idMap.ContainsKey(id))
            {
                Debug.LogError($"Cannot find {id}");
                return null;
            }
            return _idMap[id];
        }

        protected virtual void OnEnable()
        {
            if (_idMap.ContainsKey(Id))
                Debug.LogError($"Duplicate id {Id}");
            _idMap[Id] = this;
        }

        public override string ToString()
        {
            return $"{base.ToString()}, {nameof(Id)}: {Id}, {nameof(Name)}: {Name}";
        }
    }
}