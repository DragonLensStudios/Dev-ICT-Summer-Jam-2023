using System;
using System.Collections.Generic;
using DLS.Achievements;
using DLS.Core;
using DLS.Core.Data_Persistence;
using DLS.Game.Enums;
using DLS.Game.Messages;
using DLS.Game.PLayers;
using DLS.Game.Utilities;
using DLS.Utilities;
using UnityEngine;

namespace DLS.Game.Managers
{
    public class PlayerManager : MonoBehaviour
    {
        public static PlayerManager Instance { get; private set; }

        [field: SerializeField] public PlayerController Player { get; private set; }

        private void OnEnable()
        {
            MessageSystem.MessageManager.RegisterForChannel<PlayerDataMessage>(MessageChannels.Player, UpdatePlayerData);
        }

        private void OnDisable()
        {
            MessageSystem.MessageManager.UnregisterForChannel<PlayerDataMessage>(MessageChannels.Player, UpdatePlayerData);
        }

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
            Player = FindFirstObjectByType<PlayerController>(FindObjectsInactive.Include);

        }

        private void Start()
        {
            if (Player == null)
            {
                Player = FindFirstObjectByType<PlayerController>(FindObjectsInactive.Include);
            }

            // Player.ID = DataPersistenceManager.instance.dataHandler.GetMostRecentlyUpdatedPlayer().playerID;
            // Player.ObjectName = DataPersistenceManager.instance.dataHandler.GetMostRecentlyUpdatedPlayer().playerName;
            // MessageSystem.MessageManager.SendImmediate(MessageChannels.Saves, new SaveLoadMessage(Player.ID, Player.ObjectName, SaveOperation.Load));
        }

        public bool UpdatePlayer(SerializableGuid playerId, string playerName, Vector3 playerPosition, float playerMovementSpeed, string currentLevelName, List<PlayerAchievementProgress> playerAchievementProgresses, List<SavedGameObject> savedGameObjects)
        {
            if(Player == null) return false;
            Player.ID = playerId;
            Player.ObjectName = playerName;
            Player.transform.position = playerPosition;
            Player.MoveSpeed = playerMovementSpeed;
            Player.CurrentLevelName = currentLevelName;
            Player.AchievementProgressList = playerAchievementProgresses;
            Player.SavedGameObjects = savedGameObjects;
            return true;
        }
        
        private void UpdatePlayerData(MessageSystem.IMessageEnvelope message)
        {
            if(!message.Message<PlayerDataMessage>().HasValue) return;
            var data = message.Message<PlayerDataMessage>().Value;
            UpdatePlayer(data.PLayerID, data.PlayerName, data.PlayerPosition, data.PlayerSpeed, data.CurrentLevelName, data.AchievementProgressList, data.SavedGameObjects);
        }
    }
}