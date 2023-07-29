using System;
using System.Collections.Generic;
using DLS.Achievements;
using DLS.Core.Data_Persistence;
using DLS.Game.Utilities;
using UnityEngine;

namespace DLS.Game.Messages
{
    public struct PlayerDataMessage
    {
        public SerializableGuid PLayerID { get; }
        public string PlayerName { get; }
        public Vector3 PlayerPosition { get; }
        public float PlayerSpeed { get; }
        public SerializableGuid CurrentLevelID { get; }
        public string CurrentLevelName { get; }

        public List<PlayerAchievementProgress> AchievementProgressList { get; }
        
        public List<SavedGameObject> SavedGameObjects { get; }

        public PlayerDataMessage(SerializableGuid pLayerID, string playerName, Vector3 playerPosition, float playerSpeed, SerializableGuid currentLevelID, string currentLevelName, List<PlayerAchievementProgress> achievementProgressList, List<SavedGameObject> savedGameObjects)
        {
            PLayerID = pLayerID;
            PlayerName = playerName;
            PlayerPosition = playerPosition;
            PlayerSpeed = playerSpeed;
            CurrentLevelID = currentLevelID;
            CurrentLevelName = currentLevelName;
            AchievementProgressList = achievementProgressList;
            SavedGameObjects = savedGameObjects;
        }
    }
}