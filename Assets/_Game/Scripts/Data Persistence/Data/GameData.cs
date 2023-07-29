using System;
using System.Collections.Generic;
using DLS.Achievements;
using DLS.Game.Levels;
using DLS.Game.Utilities;
using UnityEngine;
using UnityEngine.Serialization;

namespace DLS.Core.Data_Persistence
{
    [System.Serializable]
    public class GameData
    {
        public SerializableGuid playerID;
        public string playerName;
        public float playerMoveSpeed;
        public string currentLevelName;
        public SerializableGuid currentLevelID;
        public DateTime lastUpdated;
        public Vector3 playerPosition;
        public Vector2 lastMovementPosition;
        public List<PlayerAchievementProgress> playerAchievements;
        public List<SavedGameObject> savedGameObjects = new ();

        public GameData()
        {
            playerID = new(Guid.Empty);
            playerName = "";
            playerMoveSpeed = 0f;
            currentLevelName = "";
            currentLevelID = new SerializableGuid(Guid.Empty);
            playerPosition = Vector3.zero;
            lastMovementPosition = Vector2.zero;
            playerAchievements = new List<PlayerAchievementProgress>();
            savedGameObjects = new List<SavedGameObject>();
        }
    }
}
