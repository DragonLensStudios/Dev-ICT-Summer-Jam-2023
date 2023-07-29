using System.Collections.Generic;
using DLS.Core.Data_Persistence;
using UnityEngine;

namespace DLS.Game.Utilities
{
    [System.Serializable]
    public class ObjectList : IID
    {
        [field: SerializeField] public GameObject gameObject { get; }
        
        [field: SerializeField] public SerializableGuid LevelID { get; set; }
        [field: SerializeField] public SerializableGuid PrefabID { get; set; }
        [field: SerializeField] public SerializableGuid ID { get; set; }
        [field: SerializeField] public string ObjectName { get; set; }
        [field: SerializeField] public List<GameObject> GameObjects { get; set; }
    }
}