using UnityEngine;
using UnityEngine.AddressableAssets;

namespace DLS.Core.Data_Persistence.Asset_Management.Asset_References
{
    [System.Serializable]
    public class AssetReferenceGameObject : AssetReferenceT<GameObject>
    {
        public AssetReferenceGameObject(string guid) : base(guid) { }
    }
}