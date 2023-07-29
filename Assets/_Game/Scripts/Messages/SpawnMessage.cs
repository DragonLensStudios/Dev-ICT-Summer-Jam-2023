using System.Collections.Generic;
using System.Linq;
using DLS.Game.Enums;
using DLS.Game.Utilities;
using JetBrains.Annotations;
using UnityEngine;

namespace DLS.Game.Messages
{
    public struct SpawnMessage
    {
        public GameObject? ObjectToSpawn { get; }
        public List<GameObject>? SpawnGameObjects { get; }
        public SpawnOperation SpawnOperation { get; }
        public List<Vector3>? SpawnPositions { get; }
        public bool? UseSpawningBounds { get; }
        public Vector3? MinBounds { get; }
        public Vector3? MaxBounds { get; }
        public bool? UseRandomSpawnObjects { get; }
        public int? AmountToSpawnMin { get; }
        public int? AmountToSpawnMax { get; }
        public Vector3? ObjectDetectRadius { get; }
        public Vector3? ObjectSpawnOffset { get; }
        public bool? UseTilemapToPopulateAvailableSpawnLocations { get; }
        public SerializableGuid? TileMapToDetectID { get; }
        public SerializableGuid? TilemapToSpawnInsideID { get; }
        public bool? UseObjectPool { get; }
        public int? ObjectPoolMinCapacity { get; }
        public int? ObjectPoolMaxCapacity { get; }

        public SpawnMessage(SpawnOperation spawnOperation, List<GameObject> spawnGameObjects = null, List<Vector3> spawnPositions = null, bool? useSpawningBounds = null, Vector3? minBounds = null, Vector3? maxBounds = null, int? amountToSpawnMin = null, int? amountToSpawnMax = null, bool? useRandomSpawnObjects = null, Vector3? objectDetectRadius = null, Vector3? objectSpawnOffset = null, bool? useTilemapToPopulateAvailableSpawnLocations = null, [CanBeNull] SerializableGuid tileMapToDetectID = null, [CanBeNull] SerializableGuid tilemapToSpawnInsideID = null, bool? useObjectPool = null, int? objectPoolMinCapacity = null, int? objectPoolMaxCapacity = null )
        {
            SpawnOperation = spawnOperation;
            ObjectToSpawn = spawnGameObjects?.FirstOrDefault();
            SpawnGameObjects = spawnGameObjects;
            SpawnPositions = spawnPositions;
            UseSpawningBounds = useSpawningBounds;
            MinBounds = minBounds;
            MaxBounds = maxBounds;
            AmountToSpawnMin = amountToSpawnMin;
            AmountToSpawnMax = amountToSpawnMax;
            ObjectDetectRadius = objectDetectRadius;
            ObjectSpawnOffset = objectSpawnOffset;
            UseTilemapToPopulateAvailableSpawnLocations = useTilemapToPopulateAvailableSpawnLocations;
            UseRandomSpawnObjects = useRandomSpawnObjects;
            TileMapToDetectID = tileMapToDetectID;
            TilemapToSpawnInsideID = tilemapToSpawnInsideID;
            UseObjectPool = useObjectPool;
            ObjectPoolMinCapacity = objectPoolMinCapacity;
            ObjectPoolMaxCapacity = objectPoolMaxCapacity;
        }
        
        public SpawnMessage(SpawnOperation spawnOperation, GameObject spawnGameObject = null,  List<Vector3> spawnPositions = null, bool? useSpawningBounds = null, Vector3? minBounds = null, Vector3? maxBounds = null, int? amountToSpawnMin = null, int? amountToSpawnMax = null, bool? useRandomSpawnObjects = null, Vector3? objectDetectRadius = null, Vector3? objectSpawnOffset = null, bool? useTilemapToPopulateAvailableSpawnLocations = null, [CanBeNull] SerializableGuid tileMapToDetectID = null, [CanBeNull] SerializableGuid tilemapToSpawnInsideID = null, bool? useObjectPool = null, int? objectPoolMinCapacity = null, int? objectPoolMaxCapacity = null)
        {
            SpawnOperation = spawnOperation;
            ObjectToSpawn = spawnGameObject;
            SpawnGameObjects = new List<GameObject> { spawnGameObject };
            SpawnPositions = spawnPositions;
            UseSpawningBounds = useSpawningBounds;
            MinBounds = minBounds;
            MaxBounds = maxBounds;
            AmountToSpawnMin = amountToSpawnMin;
            AmountToSpawnMax = amountToSpawnMax;
            ObjectDetectRadius = objectDetectRadius;
            ObjectSpawnOffset = objectSpawnOffset;
            UseTilemapToPopulateAvailableSpawnLocations = useTilemapToPopulateAvailableSpawnLocations;
            UseRandomSpawnObjects = useRandomSpawnObjects;
            TileMapToDetectID = tileMapToDetectID;
            TilemapToSpawnInsideID = tilemapToSpawnInsideID;
            UseObjectPool = useObjectPool;
            ObjectPoolMinCapacity = objectPoolMinCapacity;
            ObjectPoolMaxCapacity = objectPoolMaxCapacity;
        }
        
    }
}