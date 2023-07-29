using DLS.Game.Enums;
using DLS.Game.Messages;
using DLS.Game.Utilities;
using DLS.Utilities;
using UnityEngine;

namespace DLS.Game.Levels
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "New Level", menuName = "DLS/Game/Levels")]
    public class Level : ScriptableObject
    {
        [field: SerializeField] public SerializableGuid ID { get; set; }
        [field: SerializeField] public string LevelName { get; set; }
        [field: SerializeField] public GameObject LevelPrefab { get; set; }
        [field: SerializeField] public Vector2 PlayerSpawnPosition { get; set; }
        [field: SerializeField] public LevelState LevelState { get; set; }

        public GameObject spawnedLevel;

        public bool Enter()
        {
            LevelState = LevelState.Loading;
            spawnedLevel = Instantiate(LevelPrefab);
            spawnedLevel.name = LevelName;
            LevelState = LevelState.Loaded;
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Level, new LevelMessage(ID, LevelName, LevelState, PlayerSpawnPosition));
            return true;
        }

        public bool Exit()
        {
            if (spawnedLevel == null) return true;
            Destroy(spawnedLevel);
            LevelState = LevelState.Unloaded;
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Level, new LevelMessage(ID, LevelName, LevelState, PlayerSpawnPosition));
            return true;
        }
    }
}