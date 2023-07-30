using System;
using System.Collections.Generic;
using DLS.Game.Enums;
using DLS.Game.Messages;
using DLS.Game.Utilities;
using DLS.Utilities;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace DLS.Game.Spawning
{

    public class SpawningTrigger : MonoBehaviour
    {
        [Tooltip("Single object to spawn")]
        [field: SerializeField] public GameObject SpawnObject { get; set; }
        [Tooltip("List of game objects to spawn.")]
        [field: SerializeField] public List<GameObject> SpawnableObjects { get; set; }
        [Tooltip("When true will use Spawnable Objects to spawn rather than Spawn Object")]
        [field: SerializeField] public bool SpawnMultiple { get; set; }

        [Tooltip("List of available locations for spawning objects.")]
        [field: SerializeField] public List<Vector3> AvailableSpawnLocations { get; set; }
        [Tooltip("Minimum amount of objects to spawn.")]
        [field: SerializeField] public int AmountToSpawnMin { get; set; }
        [Tooltip("Maximum amount of objects to spawn.")]
        [field: SerializeField] public int AmountToSpawnMax { get; set; }
        [Tooltip("Radius for object detection around spawn points.")]
        [field: SerializeField] public Vector3 ObjectDetectRadius { get; set; }
        [Tooltip("The offset for object spawning")]
        [field: SerializeField] public Vector3 ObjectSpawnOffset { get; set; }
        [Tooltip("If true, objects are chosen randomly from the ObjectsToSpawn list for spawning.")]
        [field: SerializeField] public bool UseRandomSpawnObjects { get; set; }
        [Tooltip("When true will spawn objects inside the bounds defined.")]
        [field: SerializeField] public bool UseSpawningBounds { get; set; }
        [Tooltip("When UseSpawningBounds is true this will show a gizmo in the editor with this color.")]
        public Color BoundsGizmoColor = Color.green;
        [Tooltip("This the the min bounds for spawning objects")]
        [field: SerializeField] public Vector3 MinBounds { get; set; }
        [Tooltip("This the the max bounds for spawning objects")]
        [field: SerializeField] public Vector3 MaxBounds { get; set; }
        [Tooltip("When true it will use the Tilemap found with the Tilemap To Detect Key as the Tilemap to check for available spawn locations")]
        [field: SerializeField] public bool UseTilemapToPopulateAvailableSpawnLocations { get; set; }
        [Tooltip("ID used to find Tilemap that us used for finding available spawn points.")]
        [field: SerializeField] public SerializableGuid TilemapToDetectID { get; set; }
        [Tooltip("ID used to find Tilemap that us used as the parent for spawning")]
        [field: SerializeField] public SerializableGuid TilemapToSpawnInsideID { get; set; }
        [Tooltip("Use object pooling for efficiency.")]
        [field: SerializeField] public bool UseObjectPool { get; set; }
        [Tooltip("Minimum capacity of the object pool.")]
        [field: SerializeField] public int ObjectPoolMinCapacity { get; set; }
        [Tooltip("Maximum capacity of the object pool.")]
        [field: SerializeField] public int ObjectPoolMaxCapacity { get; set; }
        
        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.CompareTag("Player"))
            {
                MessageSystem.MessageManager.SendImmediate(MessageChannels.Spawning,
                    SpawnMultiple
                        ? new SpawnMessage(SpawnOperation.SpawnAll,
                            spawnGameObjects: SpawnableObjects,
                            spawnPositions: AvailableSpawnLocations,
                            useSpawningBounds: UseSpawningBounds,
                            minBounds: MinBounds, maxBounds: MaxBounds,
                            amountToSpawnMin: AmountToSpawnMin,
                            amountToSpawnMax: AmountToSpawnMax,
                            useRandomSpawnObjects: UseRandomSpawnObjects,
                            objectDetectRadius: ObjectDetectRadius,
                            objectSpawnOffset: ObjectSpawnOffset,
                            useTilemapToPopulateAvailableSpawnLocations: UseTilemapToPopulateAvailableSpawnLocations,
                            tileMapToDetectID: TilemapToDetectID,
                            tilemapToSpawnInsideID: TilemapToSpawnInsideID,
                            useObjectPool: UseObjectPool,
                            objectPoolMinCapacity: ObjectPoolMinCapacity,
                            objectPoolMaxCapacity: ObjectPoolMaxCapacity
                        )
                        : new SpawnMessage(SpawnOperation.SpawnSingle,
                            spawnGameObject: SpawnObject,
                            spawnPositions: AvailableSpawnLocations,
                            useSpawningBounds: UseSpawningBounds,
                            minBounds: MinBounds, maxBounds: MaxBounds,
                            amountToSpawnMin: AmountToSpawnMin,
                            amountToSpawnMax: AmountToSpawnMax,
                            useRandomSpawnObjects: UseRandomSpawnObjects,
                            objectDetectRadius: ObjectDetectRadius,
                            objectSpawnOffset: ObjectSpawnOffset,
                            useTilemapToPopulateAvailableSpawnLocations: UseTilemapToPopulateAvailableSpawnLocations,
                            tileMapToDetectID: TilemapToDetectID,
                            tilemapToSpawnInsideID: TilemapToSpawnInsideID,
                            useObjectPool: UseObjectPool,
                            objectPoolMinCapacity: ObjectPoolMinCapacity,
                            objectPoolMaxCapacity: ObjectPoolMaxCapacity
                        ));
            }
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