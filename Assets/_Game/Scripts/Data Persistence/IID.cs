using DLS.Game.Utilities;
using UnityEngine;

namespace DLS.Core.Data_Persistence
{
    public interface IID
    {
        GameObject gameObject { get ; } 
        SerializableGuid PrefabID { get; set; }
        SerializableGuid LevelID { get; set; }
        SerializableGuid ID { get; set; }
        string ObjectName { get; set; }
    }
}