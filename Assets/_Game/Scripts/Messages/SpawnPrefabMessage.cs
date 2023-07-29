using DLS.Game.Enums;
using DLS.Game.Utilities;
using UnityEditor.AddressableAssets.Build.Layout;
using UnityEngine;

namespace DLS.Game.Messages
{
    public struct SpawnPrefabMessage
    {
        public string PrefabKey { get; }
        public string Name { get; }
        public TransformData Transform { get; }
        public SerializableGuid ObjectID { get;}
        
        public SpawnPrefabMessage(string prefabKey, string name, TransformData transform, SerializableGuid objectID = null)
        {
            PrefabKey = prefabKey;
            Name = name;
            Transform = transform;
            ObjectID = objectID;
        }
    }
}