using UnityEngine;
using UnityEngine.AddressableAssets;

namespace DLS.Core.Data_Persistence.Asset_Management.Asset_References
{
    [System.Serializable]
    public class AssetReference : AssetReferenceT<Object>
    {
        public AssetReference(string guid) : base(guid) { }
    }
}