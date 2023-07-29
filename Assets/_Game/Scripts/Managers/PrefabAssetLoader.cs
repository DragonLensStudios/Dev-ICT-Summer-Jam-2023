using UnityEngine;
using System;
using System.Linq;
using System.Threading.Tasks;
using DLS.Core.Data_Persistence;
using DLS.Core.Data_Persistence.Asset_Management;
using DLS.Core.Data_Persistence.Asset_Management.Asset_References;
using DLS.Core.Data_Persistence.Extensions;
using DLS.Game.Enums;
using DLS.Game.Messages;
using DLS.Utilities;
using UnityEditor.AddressableAssets.Build.Layout;
using UnityEngine.Serialization;
using UnityEngine.U2D;

namespace DLS.Game.Managers
{
    public class PrefabAssetLoader : MonoBehaviour
    {
        [field: SerializeField] public int RetryAmount { get; set; } = 5;
        // Singleton instance
        public static PrefabAssetLoader Instance { get; private set; }

        private int retryCount = 0;

        public PrefabReferences PrefabReferences;

        private void Awake()
        {
            // Singleton pattern
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnEnable()
        {
            MessageSystem.MessageManager.RegisterForChannel<SpawnPrefabMessage>(MessageChannels.Spawning, SpawnPrefabMessageHandler);
        }

        private void OnDisable()
        {
            MessageSystem.MessageManager.UnregisterForChannel<SpawnPrefabMessage>(MessageChannels.Spawning, SpawnPrefabMessageHandler);
        }

        // Load and instantiate GameObject by key
        public async Task<GameObject> SpawnPrefab(string key, string name, Vector3 position, Quaternion rotation, Vector3 scale, Transform parent = null)
        {
            if (!PrefabReferences.PrefabAssetReferences.ContainsKey(key)) return null;
            
            AssetReferenceGameObject assetReference = PrefabReferences.PrefabAssetReferences[key];
            GameObject go = await assetReference.InstantiateAsync(position, rotation, parent).Task;
        
            if (go == null)
            {
                if (retryCount > RetryAmount) return go;
                Debug.LogWarning($"Something went wrong trying to spawn prefab trying again. Try: {retryCount + 1} / {RetryAmount + 1}");
                go = await SpawnPrefab(key, name, position, rotation, scale, parent);
                retryCount++;
            }
            else
            {
                go.name = name;
                go.transform.localScale = scale;
                await MessageSystem.MessageManager.SendImmediateAsync(MessageChannels.Spawning, new GameObjectMessage(go));

                retryCount = 0;
            }

            return go;

        }
        
        private async void SpawnPrefabMessageHandler(MessageSystem.IMessageEnvelope message)
        {
            if (!message.Message<SpawnPrefabMessage>().HasValue) return;
            var data = message.Message<SpawnPrefabMessage>().Value;
            var objects = FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None).Select(x=> x.GetComponent<IID>());
            Transform parentTransform = null;
            foreach (var obj in objects)
            {
                var id = obj.ID;
                if (id.Guid == Guid.Empty ) continue;
                if (id.Equals(data.Transform.parentID))
                {
                    parentTransform = obj.gameObject.transform;
                }
            }
            var spawnedObj = await SpawnPrefab(data.PrefabKey, data.Name, data.Transform.localPosition, data.Transform.localRotation, data.Transform.localScale, parentTransform);
        }
    }
}