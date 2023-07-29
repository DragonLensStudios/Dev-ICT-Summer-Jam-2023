using System;
using System.Collections.Generic;
using System.Linq;
using DLS.Achievements;
using DLS.Core.Data_Persistence;
using DLS.Game.Enums;
using DLS.Game.GameStates;
using DLS.Game.Levels;
using DLS.Game.Managers;
using DLS.Game.Messages;
using DLS.Game.PLayers;
using DLS.Game.Utilities;
using DLS.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DLS.Core.UI
{
    public class NewGameController : MonoBehaviour
    {
        [SerializeField] private Level level;
        [SerializeField] private Vector3 playerPosition = Vector3.zero;
        [SerializeField] private float playerMoveSpeed = 3f;
        [SerializeField] private List<PlayerAchievementProgress> playerAchievementProgresses = new();
        [SerializeField] private List<SavedGameObject> savedGameObjects = new();
        [SerializeField] private GameState state;
        [SerializeField] private Button startGameButton;
        [SerializeField] private TMP_InputField nameInputField;

        private PlayerController player;

        private void Start()
        {
            player = PlayerManager.Instance.Player;
            if (player != null)
            {
                player.ID = new SerializableGuid(Guid.NewGuid());
                player.SetAchievementProgress();
                playerAchievementProgresses = player.AchievementProgressList;
                startGameButton.interactable = !string.IsNullOrWhiteSpace(nameInputField.text);
                player.SavedGameObjects = new();
            }
        }

        public void SetPlayer(string name)
        {
            if (player == null) return;
            startGameButton.interactable = !string.IsNullOrWhiteSpace(name);
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Player, new PlayerDataMessage(player.ID, name, playerPosition, playerMoveSpeed, level.ID, level.LevelName, playerAchievementProgresses, savedGameObjects));
        }
        public void LoadLevel()
        {
            if(player == null) return;
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Level, new LevelMessage(level.ID, level.LevelName, LevelState.Loading, playerPosition));
            MessageSystem.MessageManager.SendImmediate(MessageChannels.GameFlow, new GameStateMessage(state));
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Saves, new SaveLoadMessage(player.ID, player.ObjectName, SaveOperation.Save));
        }
    }
}