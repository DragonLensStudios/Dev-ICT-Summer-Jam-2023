using DLS.Core.Data_Persistence;
using DLS.Game.Utilities;
using UnityEngine;

public class ObjectController : MonoBehaviour, IID
{
    [field: SerializeField] public SerializableGuid LevelID { get; set; }
    [field: SerializeField] public SerializableGuid PrefabID { get; set; }
    [field: SerializeField] public SerializableGuid ID { get; set; }
    [field: SerializeField] public string ObjectName { get; set; }
}
