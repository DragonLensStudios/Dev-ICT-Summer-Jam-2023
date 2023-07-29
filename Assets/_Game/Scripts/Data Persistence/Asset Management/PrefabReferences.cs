using DLS.Core.Data_Persistence.Asset_Management.Asset_References;
using DLS.Game.Utilities;
using UnityEngine;

namespace DLS.Core.Data_Persistence.Asset_Management
{
    [CreateAssetMenu(fileName = "Prefab Asset Management", menuName = "DLS/Asset Management/Prefab References")]
    public class PrefabReferences : ScriptableObject
    {
        public SerializableDictionary<string, AssetReferenceGameObject> PrefabAssetReferences;
    }
}