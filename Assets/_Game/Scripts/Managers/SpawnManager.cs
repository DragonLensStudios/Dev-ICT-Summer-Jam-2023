using System;
using System.Collections.Generic;
using System.Linq;
using DLS.Core.Data_Persistence;
using DLS.Core.Data_Persistence.Extensions;
using DLS.Game.Enums;
using DLS.Game.Messages;
using DLS.Game.Tilemaps;
using DLS.Game.Utilities;
using DLS.Utilities;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace DLS.Game.Managers
{
    public class SpawnManager : MonoBehaviour, IDataPersistence
    {
        /// <summary>
        /// Singleton instance of the SpawnManager.
        /// </summary>
        public static SpawnManager Instance { get; private set; }
    
        [Tooltip("When UseSpawningBounds is true this will show a gizmo in the editor with this color.")]
        public Color BoundsGizmoColor = Color.green;
        
        [Tooltip("Level ID Guid")]
        [field: SerializeField] public SerializableGuid LevelID { get; set; }
        
        [Tooltip("Prefab Object ID Guid")]
        [field: SerializeField] public SerializableGuid PrefabID { get; set; }
        [Tooltip("Object ID Guid")]
        [field: SerializeField] public SerializableGuid ID { get; set; }
        [Tooltip("Object Name")]
        [field: SerializeField] public string ObjectName { get; set; }
        [Tooltip("Single object to spawn")]
        public GameObject ObjectToSpawn;
        [Tooltip("List of game objects to spawn.")]
        public List<GameObject> ObjectsToSpawn;
        [Tooltip("If true, objects are chosen randomly from the ObjectsToSpawn list for spawning.")]
        public bool UseRandomSpawnObjects;
        [Tooltip("Minimum amount of objects to spawn.")]
        public int AmountToSpawnMin;
        [Tooltip("Maximum amount of objects to spawn.")]
        public int AmountToSpawnMax;
        [Tooltip("When true will spawn objects inside the bounds defined.")]
        public bool UseSpawningBounds;
        [Tooltip("This the the min bounds for spawning objects")]
        public Vector3 MinBounds;
        [Tooltip("This the the max bounds for spawning objects")]
        public Vector3 MaxBounds;
        [Tooltip("Radius for object detection around spawn points.")]
        public Vector3 ObjectDetectRadius;
        [Tooltip("The offset for object spawning")]
        public Vector3 ObjectSpawnOffset;
        [Tooltip("When true it will use the Tilemap found with the Tilemap To Detect ID as the Tilemap to check for available spawn locations")]
        public bool UseTilemapToPopulateAvailableSpawnLocations;
        [Tooltip("ID used to find Tilemap that us used for finding available spawn points.")]
        public SerializableGuid TilemapToDetectID;
        [Tooltip("Tilemap to use for detecting available spawn points.")]
        public Tilemap TilemapToDetectAvailableSpawnPoints;
        [Tooltip("ID used to find Tilemap that us used as the parent for spawning")]
        public SerializableGuid TilemapToSpawnInsideID;
        [Tooltip("Tilemap to use for the spawn parent")]
        public Tilemap TilemapToSpawnInside;
        [Tooltip("Use object pooling for efficiency.")]
        public bool UseObjectPool;
        [Tooltip("Minimum capacity of the object pool.")]
        public int ObjectPoolMinCapacity;
        [Tooltip("Maximum capacity of the object pool.")]
        public int ObjectPoolMaxCapacity;
        [Tooltip("List of available locations for spawning objects.")]
        public List<Vector3> AvailableSpawnLocations;
        [Tooltip("Storage of currently active spawned objects.")]
        public List<ObjectList> ActiveObjects;
        [Tooltip("Pool of game objects for efficient spawning/despawning.")]
        public ObjectPool<GameObject> Pool;
        [Tooltip("Queue of game objects to be spawned.")]
        private Queue<GameObject> SpawnObjectsQueue = new ();

        private GameData savedData;

        /// <summary>
        /// Ensures that only one instance of SpawnManager exists in the scene.
        /// </summary>
        private void Awake()
        {
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

        /// <summary>
        /// Registers for spawn messages when the object is enabled.
        /// </summary>
        private void OnEnable()
        {
            MessageSystem.MessageManager.RegisterForChannel<SpawnMessage>(MessageChannels.Spawning, HandleSpawnMessage);
            MessageSystem.MessageManager.RegisterForChannel<LevelMessage>(MessageChannels.Level, HandleLevelMessage);
        }

        /// <summary>
        /// Unregisters for spawn messages when the object is disabled.
        /// </summary>
        private void OnDisable()
        {
        
            MessageSystem.MessageManager.UnregisterForChannel<SpawnMessage>(MessageChannels.Spawning, HandleSpawnMessage);
            MessageSystem.MessageManager.UnregisterForChannel<LevelMessage>(MessageChannels.Level, HandleLevelMessage);
        }

        /// <summary>
        /// Sets up the object pool and queue for spawning objects on game start.
        /// </summary>
        private void Start()
        {
            SetupPool();
            SpawnObjectsQueue.Clear();
            for (int i = 0; i < ObjectsToSpawn.Count; i++)
            {
                if (!SpawnObjectsQueue.Contains(ObjectsToSpawn[i]))
                {
                    SpawnObjectsQueue.Enqueue(ObjectsToSpawn[i]);
                }
            }

            var tileMapController = FindFirstObjectByType<TilemapController>(FindObjectsInactive.Include);
            if (tileMapController == null) return;
        
            if (tileMapController.Tilemaps.TryGetValue(TilemapToSpawnInsideID, out var parentTilemap))
            {
                TilemapToSpawnInside = parentTilemap;
            }
            if (!UseTilemapToPopulateAvailableSpawnLocations) return;
            if(tileMapController.Tilemaps.TryGetValue(TilemapToDetectID, out var detectTilemap))
            {
                TilemapToDetectAvailableSpawnPoints = detectTilemap;
            }
        }
    
        /// <summary>
        /// Initializes the object pool if UseObjectPool is set to true.
        /// </summary>
        private void SetupPool()
        {
            if (UseObjectPool)
            {
                Pool = new ObjectPool<GameObject>(OnPoolObjectCreate, OnPoolObjectGet, OnPoolObjectRelease, OnPoolObjectDestroy, false, ObjectPoolMinCapacity, ObjectPoolMaxCapacity);
            }
        }
    
        /// <summary>
        /// Creates an object to be added to the pool.
        /// </summary>
        private GameObject OnPoolObjectCreate()
        {
            Vector3 randomPos = Vector3.zero;
            GameObject objectToSpawn = ObjectToSpawn;
        
            if(AvailableSpawnLocations.Count > 0)
            {
                randomPos = AvailableSpawnLocations[Random.Range(0, AvailableSpawnLocations.Count)];
            }

            if (SpawnObjectsQueue.Count > 0 && ObjectsToSpawn.Count > 0)
            {
                objectToSpawn = Instantiate(UseRandomSpawnObjects ? ObjectsToSpawn[Random.Range(0, ObjectsToSpawn.Count)] : SpawnObjectsQueue.Dequeue(), randomPos, Quaternion.identity);
            }
            else if(ObjectToSpawn != null)
            {
                objectToSpawn = Instantiate(objectToSpawn, randomPos, Quaternion.identity);
            }
        
            objectToSpawn.transform.parent = transform;
            AvailableSpawnLocations.Remove(randomPos);
            
            var objList = new ObjectList
            {
                ID = gameObject.GetObjectInstanceID(),
                PrefabID = gameObject.GetObjectPrefabID(),
                LevelID = LevelManager.Instance.CurrentLevel.ID,
                ObjectName = LevelManager.Instance.CurrentLevel.LevelName,
                GameObjects = new List<GameObject>()
            };
            

            var levelActiveObjects = ActiveObjects.FirstOrDefault(x => x.LevelID.Equals(LevelManager.Instance.CurrentLevel.ID));
            if (levelActiveObjects == null && ActiveObjects.Count <= 0)
            {
                ActiveObjects.Add(objList);
                var activeObject = ActiveObjects.FirstOrDefault(x => x.LevelID.Equals(LevelManager.Instance.CurrentLevel.ID));
                if (activeObject != null)
                {
                    activeObject.GameObjects.Add(objectToSpawn);
                }

                levelActiveObjects = objList;
            }

            if (levelActiveObjects == null) return objectToSpawn;
            if (!levelActiveObjects.ID.Equals(objList.ID))
            {
                levelActiveObjects.GameObjects.Add(objectToSpawn);
            }

            return objectToSpawn;
        }
    
        /// <summary>
        /// Prepares the object for use from the pool.
        /// </summary>
        private void OnPoolObjectGet(GameObject obj)
        {
            if(AvailableSpawnLocations.Count > 0)
            {
                obj.transform.position = AvailableSpawnLocations[Random.Range(0, AvailableSpawnLocations.Count)];
            }

            obj.SetActive(true);
        }
    
        /// <summary>
        /// Prepares the object to be returned to the pool.
        /// </summary>
        private void OnPoolObjectRelease(GameObject obj)
        {
            obj.SetActive(false);
        }
    
        /// <summary>
        /// Destroys an object when it is removed from the pool.
        /// </summary>
        private void OnPoolObjectDestroy(GameObject obj)
        {
            var levelActiveObjects = ActiveObjects.FirstOrDefault(x => x.ID.Guid == LevelManager.Instance.CurrentLevel.ID.Guid);
            if (levelActiveObjects == null) return;
            if (levelActiveObjects.GameObjects.Contains(obj))
            {
                levelActiveObjects.GameObjects.Remove(obj);
            }

            Destroy(obj);
        }
    
        /// <summary>
        /// Despawns all active objects.
        /// </summary>
        public void DespawnObjects()
        {
            var levelActiveObjects = ActiveObjects.FirstOrDefault(x => x.ID.Guid == LevelManager.Instance.CurrentLevel.ID.Guid);
            if (levelActiveObjects == null) return;

            for (int i = 0; i < levelActiveObjects.GameObjects.Count; i++)
            {
                var obj = levelActiveObjects.GameObjects[i];
                DespawnOject(obj);
            }
        }

        /// <summary>
        /// Spawns a specific object at a location from the available spawn locations.
        /// </summary>
        public void SpawnObject(GameObject obj, params Vector3[] spawnLocations)
        {
            AddSpawnLocations(spawnLocations);
        
            if (AvailableSpawnLocations.Count <= 0) return;
        
            var randomPos = AvailableSpawnLocations[Random.Range(0, AvailableSpawnLocations.Count)];

            obj.SetObjectInstanceID(new SerializableGuid(Guid.NewGuid()));
            var objList = new ObjectList
            {
                ID = gameObject.GetObjectInstanceID(),
                PrefabID = gameObject.GetObjectPrefabID(),
                ObjectName = LevelManager.Instance.CurrentLevel.LevelName,
                LevelID = LevelManager.Instance.CurrentLevel.ID,
                GameObjects = new List<GameObject>()
            };
            
            if (UseObjectPool)
            {
                var poolGo = Pool.Get();
                poolGo.transform.position = randomPos;
                poolGo.transform.parent = TilemapToSpawnInside != null ? TilemapToSpawnInside.transform : transform;
                
                if (ActiveObjects is { Count: 0 })
                {
                    ActiveObjects.Add(objList);
                }
                else
                {
                    if (!ActiveObjects.Exists(x=> x.LevelID.Equals(objList.LevelID)))
                    {
                        ActiveObjects.Add(objList);
                    }
                }
                
                var levelActiveObjects = ActiveObjects.FirstOrDefault(x => x.LevelID.Equals(LevelManager.Instance.CurrentLevel.ID));
                if (levelActiveObjects == null) return;
                if (!levelActiveObjects.GameObjects.Contains(poolGo))
                {
                    levelActiveObjects.GameObjects.Add(poolGo);
                }
                if (!SpawnObjectsQueue.Contains(poolGo))
                {
                    SpawnObjectsQueue.Enqueue(poolGo);
                }
                AvailableSpawnLocations.Remove(randomPos);
            }
            else
            {
                var spawnedObject = Instantiate(obj, randomPos, Quaternion.identity);
                var spawnedID = spawnedObject.GetComponent<IID>();
                spawnedObject.SetActive(true);
                spawnedObject.transform.parent = TilemapToSpawnInside != null ? TilemapToSpawnInside.transform : transform;
                
                var levelActiveObjects = ActiveObjects.FirstOrDefault(x => x.LevelID.Equals(LevelManager.Instance.CurrentLevel.ID));

                if (levelActiveObjects == null)
                {
                    levelActiveObjects = objList;
                    if (!levelActiveObjects.ID.Equals(spawnedID.ID))
                    {
                        levelActiveObjects.GameObjects.Add(spawnedObject);
                    }
                    ActiveObjects.Add(levelActiveObjects);
                }
                else
                {
                    if (!levelActiveObjects.GameObjects.Contains(spawnedObject))
                    {
                        levelActiveObjects.GameObjects.Add(spawnedObject);
                    }
                }

                if (!SpawnObjectsQueue.Contains(spawnedObject))
                {
                    SpawnObjectsQueue.Enqueue(spawnedObject);
                }
                AvailableSpawnLocations.Remove(randomPos);
            }
        }
    
        /// <summary>
        /// Despawns a specific object.
        /// </summary>
        public void DespawnOject(GameObject obj)
        {
            var levelActiveObjects = ActiveObjects.FirstOrDefault(x => x.ID.Guid == LevelManager.Instance.CurrentLevel.ID.Guid);
            if (levelActiveObjects == null) return;
            var existingActiveObject = levelActiveObjects.GameObjects.FirstOrDefault(x => x == obj);
            if(existingActiveObject == null) return;
            if(existingActiveObject.activeSelf != true) return;
            levelActiveObjects.GameObjects.Remove(existingActiveObject);
            
            if (UseObjectPool)
            {
                Pool.Release(obj);
            }
            else
            {
                Destroy(obj);
            }
        }
    
        /// <summary>
        /// Spawns a random amount of objects within the min and max range at available spawn locations.
        /// </summary>
        public void SpawnObjects(params Vector3[] spawnPositions)
        {
            var amountToSpawn = Random.Range(AmountToSpawnMin, AmountToSpawnMax + 1);
            for (int i = 0; i < amountToSpawn; i++)
            {
                GameObject obj = null;
                obj = UseRandomSpawnObjects ? ObjectsToSpawn[Random.Range(0, ObjectsToSpawn.Count)] : SpawnObjectsQueue.Dequeue();
                SpawnObject(obj, spawnPositions);
            }
        }
    
        /// <summary>
        /// Adds new spawn locations for objects.
        /// </summary>
        public void AddSpawnLocations(params Vector3[] spawnPositions)
        {
            AvailableSpawnLocations.Clear();
            if (spawnPositions is not { Length: 0 })
            {
                foreach (var pos in spawnPositions)
                {
                    if (UseSpawningBounds)
                    {
                        if (pos.x < MinBounds.x || pos.x > MaxBounds.x || pos.y < MinBounds.y || pos.y > MaxBounds.y)
                        {
                            continue;
                        }
                    }
                    Vector3Int localPlace = new Vector3Int((int)pos.x, (int)pos.y , (int)pos.z);
                    Vector3 place = TilemapToDetectAvailableSpawnPoints.CellToWorld(localPlace) + ObjectSpawnOffset;
                    if (TilemapToDetectAvailableSpawnPoints.HasTile(localPlace))
                    {
                        var objHere = GetObjectsAtPosition(place, ObjectDetectRadius, 0f);
                        if (!objHere.objectFound)
                        {
                            AvailableSpawnLocations.Add(place);
                        }
                        else
                        {
                            for (int i = 0; i < objHere.colliders.Count; i++)
                            {
                                var obj = objHere.colliders[i];
                                if (obj != null)
                                {
                                    Debug.Log($"Object: {obj.name} at X:{place.x} Y:{place.y} Z:{place.z}");
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (var pos in TilemapToDetectAvailableSpawnPoints.cellBounds.allPositionsWithin)
                {
                    if (UseSpawningBounds)
                    {
                        if (pos.x < MinBounds.x || pos.x > MaxBounds.x || pos.y < MinBounds.y || pos.y > MaxBounds.y)
                        {
                            continue;
                        }
                    }
                    Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);
                    Vector3 place = TilemapToDetectAvailableSpawnPoints.CellToWorld(localPlace) + ObjectSpawnOffset;
                    if (TilemapToDetectAvailableSpawnPoints.HasTile(localPlace))
                    {
                        var objHere = GetObjectsAtPosition(place, ObjectDetectRadius, 0f);
                        if (!objHere.objectFound)
                        {
                            AvailableSpawnLocations.Add(place);
                        }
                        else
                        {
                            for (int i = 0; i < objHere.colliders.Count; i++)
                            {
                                var obj = objHere.colliders[i];
                                if (obj != null)
                                {
                                    Debug.Log($"Object: {obj.name} at X:{place.x} Y:{place.y} Z:{place.z}");
                                }
                            }
                        }
                    }
                }
            }
        }
    
        /// <summary>
        /// Detects whether there are objects at the given position within the given radius.
        /// </summary>
        public (bool objectFound, List<Collider2D> colliders) GetObjectsAtPosition(Vector2 position, Vector2 radius, float angle)
        {
            Collider2D[] intersecting = Physics2D.OverlapBoxAll(position, radius, angle);
            return (intersecting.Length != 0, intersecting.ToList());
        }
    
        /// <summary>
        /// Handles SpawnMessage and performs spawn/despawn operations based on message data.
        /// </summary>
        private void HandleSpawnMessage(MessageSystem.IMessageEnvelope message)
        {
            if (!message.Message<SpawnMessage>().HasValue) return;
            var data = message.Message<SpawnMessage>().Value;
            if (data.ObjectToSpawn != null)
            {
                ObjectToSpawn = data.ObjectToSpawn;
                SpawnObjectsQueue.Clear();
                SpawnObjectsQueue.Enqueue(ObjectToSpawn);
            }
            if (data.SpawnGameObjects is not {Count: 0})
            {
                ObjectsToSpawn = data.SpawnGameObjects;
                SpawnObjectsQueue.Clear();
                for (int i = 0; i < ObjectsToSpawn.Count; i++)
                {
                    SpawnObjectsQueue.Enqueue(ObjectsToSpawn[i]);
                }
            }
        
            if (data.AmountToSpawnMin.HasValue)
            {
                AmountToSpawnMin = data.AmountToSpawnMin.Value;
            }

            if (data.AmountToSpawnMax.HasValue)
            {
                AmountToSpawnMax = data.AmountToSpawnMax.Value;
            }

            if (data.UseSpawningBounds.HasValue)
            {
                UseSpawningBounds = data.UseSpawningBounds.Value;
            }

            if (data.MinBounds.HasValue)
            {
                MinBounds = data.MinBounds.Value;
            }
            if (data.MaxBounds.HasValue)
            {
                MaxBounds = data.MaxBounds.Value;
            }

            if (data.ObjectDetectRadius.HasValue)
            {
                ObjectDetectRadius = data.ObjectDetectRadius.Value;
            }

            if (data.ObjectSpawnOffset.HasValue)
            {
                ObjectSpawnOffset = data.ObjectSpawnOffset.Value;
            }

            if (data.UseTilemapToPopulateAvailableSpawnLocations.HasValue)
            {
                UseTilemapToPopulateAvailableSpawnLocations = data.UseTilemapToPopulateAvailableSpawnLocations.Value;
            }
        
            if (data.UseObjectPool.HasValue)
            {
                UseObjectPool = data.UseObjectPool.Value;
            }

            if (data.ObjectPoolMinCapacity.HasValue)
            {
                ObjectPoolMinCapacity = data.ObjectPoolMinCapacity.Value;
            }

            if (data.ObjectPoolMaxCapacity.HasValue)
            {
                ObjectPoolMaxCapacity = data.ObjectPoolMaxCapacity.Value;
            }

            if (data.UseRandomSpawnObjects.HasValue)
            {
                UseRandomSpawnObjects = data.UseRandomSpawnObjects.Value;
            }

            if (data.SpawnPositions?.Count > 0)
            {
                AddSpawnLocations(data.SpawnPositions?.ToArray());
            }
            else
            {
                AddSpawnLocations();
            }
            if (data.TileMapToDetectID != null)
            {
                TilemapToDetectID = data.TileMapToDetectID;
            }

            if (data.TilemapToSpawnInsideID != null)
            {
                TilemapToSpawnInsideID = data.TilemapToSpawnInsideID;
            }

            switch (data.SpawnOperation)
            {
                case SpawnOperation.SpawnSingle:
                    var spawnGo = data.ObjectToSpawn;
                    if (spawnGo != null)
                    {
                        if (data.SpawnPositions?.Count > 0)
                        {
                            SpawnObject(spawnGo, data.SpawnPositions.ToArray());
                        }
                        else
                        {
                            SpawnObject(spawnGo);
                        }
                    }
                    break;
                case SpawnOperation.SpawnAll:
                    if (data.SpawnPositions?.Count > 0)
                    {
                        SpawnObjects(data.SpawnPositions.ToArray());
                    }
                    else
                    {
                        SpawnObjects();
                    }
                    break;
                case SpawnOperation.DespawnSingle:
                    var despawnGo = data.ObjectToSpawn;
                    if (despawnGo != null)
                    {
                        DespawnOject(despawnGo);
                    }
                    break;
                case SpawnOperation.DespawnAll:
                    DespawnObjects();
                    break;
            }
        }
    
        private async void HandleLevelMessage(MessageSystem.IMessageEnvelope message)
        {
            try
            {
                if(!message.Message<LevelMessage>().HasValue) return;
                var messageData = message.Message<LevelMessage>().Value;
                if(messageData.LevelState != LevelState.Loaded) return;
                if (ActiveObjects is { Count: > 0 })
                {
                    ActiveObjects.Clear();
                }

                var tileMapController = FindFirstObjectByType<TilemapController>(FindObjectsInactive.Include);
                if (tileMapController == null) return;
            
                tileMapController.GetTilemaps();
            
                if (tileMapController.Tilemaps.TryGetValue(TilemapToSpawnInsideID, out var parentTilemap))
                {
                    TilemapToSpawnInside = parentTilemap;
                }
                if (!UseTilemapToPopulateAvailableSpawnLocations) return;
                if(tileMapController.Tilemaps.TryGetValue(TilemapToDetectID, out var detectTilemap))
                {
                    TilemapToDetectAvailableSpawnPoints = detectTilemap;
                }

                if (savedData == null || !savedData.playerID.Equals(PlayerManager.Instance.Player.ID)) return;
                
                
                foreach (var savedObj in savedData.savedGameObjects)
                { 
                    if(!savedObj.currentLevelID.Equals(LevelManager.Instance.CurrentLevel.ID)) continue;
                    
                    var gos = GameObject.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);
                
                    var goIdLookup = gos
                        .Select(go => new { GameObject = go, IID = go.GetComponent<IID>() })
                        .Where(item => item.IID != null && item.IID.ID.Guid != Guid.Empty)
                        .GroupBy(item => item.IID.ID.Guid)
                        .ToDictionary(group => group.Key, group => group.First().GameObject);

                    string key = savedObj.prefabID.Guid.ToString();

                    Transform parentTransform = null;

                    // Use the dictionary to find the parent GameObject based on the ID.
                    if (goIdLookup.TryGetValue(savedObj.transformData.parentID.Guid, out var parent))
                    {
                        if (parent != null)
                        {
                            parentTransform = parent.transform;
                        }
                    }
                    var objList = new ObjectList
                    {
                        ID = gameObject.GetObjectInstanceID(),
                        PrefabID = gameObject.GetObjectPrefabID(),
                        LevelID = LevelManager.Instance.CurrentLevel.ID,
                        ObjectName = LevelManager.Instance.CurrentLevel.LevelName,
                        GameObjects = new List<GameObject>()
                    };
                    var go = await PrefabAssetLoader.Instance.SpawnPrefab(key, savedObj.objectName, savedObj.transformData.localPosition,
                        savedObj.transformData.localRotation, savedObj.transformData.localScale, parentTransform);
                    if (go == null) continue;
                    go.SetObjectInstanceID(savedObj.instanceID);
                    go.SetObjectName(savedObj.objectName);
                    
                    if (!objList.GameObjects.Contains(go))
                    {
                        objList.GameObjects.Add(go);
                    }
                    var activeObjects = ActiveObjects.FirstOrDefault(x => x.LevelID.Equals(LevelManager.Instance.CurrentLevel.ID));
                    if (activeObjects == null)
                    {
                        ActiveObjects.Add(objList);
                        activeObjects = objList;
                    }
                    
                    if (!activeObjects.GameObjects.Contains(go))
                    {
                        activeObjects.GameObjects.Add(go);
                    }

                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
            
        }

        public void LoadData(GameData data)
        {
            savedData = data;
        }

        public void SaveData(GameData data)
        {
            if(data == null) return;
            foreach (var activeObjects in ActiveObjects)
            {
                if(activeObjects.GameObjects.Count <= 0) continue;
                foreach (var activeObject in activeObjects.GameObjects)
                {
                    if(activeObject == null) continue;
                    SerializableGuid parentID = new SerializableGuid(Guid.Empty);
                    SavedGameObject savedObj = null;
                    if (activeObject.transform.parent != null)
                    {
                        var parent = activeObject.transform.parent;
                        parentID = parent.gameObject.GetObjectInstanceID();
                    }
                    savedObj = new SavedGameObject
                    {
                        currentLevel = LevelManager.Instance.CurrentLevel.LevelName,
                        currentLevelID = LevelManager.Instance.CurrentLevel.ID,
                        prefabID = activeObject.GetObjectPrefabID(),
                        instanceID = activeObject.GetObjectInstanceID(),
                        objectName = activeObject.GetObjectName(),
                        transformData = new TransformData(activeObject.transform.position, activeObject.transform.rotation, activeObject.transform.localScale, parentID)
                    };
                    
                    
                    if (savedObj == null) continue;
                    if (!data.savedGameObjects.Exists(x => x.instanceID.Equals(savedObj.instanceID)))
                    {
                        data.savedGameObjects.Add(savedObj);
                    }
                }
            }

            savedData = data;
        }
        private void OnDrawGizmos()
        {
            if (!UseSpawningBounds) return;
            Gizmos.color = BoundsGizmoColor;
            Vector3 center = (MinBounds + MaxBounds) * 0.5f;
            Vector3 size = MaxBounds - MinBounds;
            Gizmos.DrawCube(center, size);
        }

    }
}
