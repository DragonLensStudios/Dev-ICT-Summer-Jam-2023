using System;
using System.Linq;
using DLS.Core.Data_Persistence;
using DLS.Game.Enums;
using DLS.Game.Messages;
using DLS.Utilities;
using UnityEngine;

namespace DLS.Core.UI
{
    public class LoadGameController : MonoBehaviour
    {
        public static LoadGameController Instance;
        [SerializeField] private GameObject loadPanelPrefab;
        [SerializeField] private Transform loadPanelContent;

        private void Start()
        {
            ClearAndSpawnLoadGamePanels();
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnEnable()
        {
            MessageSystem.MessageManager.RegisterForChannel<SaveLoadMessage>(MessageChannels.Saves, SaveLoadMessageHandler);
        }

        private void OnDisable()
        {
            MessageSystem.MessageManager.UnregisterForChannel<SaveLoadMessage>(MessageChannels.Saves, SaveLoadMessageHandler);
        }

        public void ClearAndSpawnLoadGamePanels()
        {
            for (int i = loadPanelContent.childCount - 1; i >= 0; i--)
            {
                Destroy(loadPanelContent.GetChild(i).gameObject);
            }

            var data = DataPersistenceManager.instance.GetAllProfilesGameData().OrderByDescending(x=> x.Value.lastUpdated);
            foreach (var gameData in data)
            {
                var save = Instantiate(loadPanelPrefab, loadPanelContent);
                var saveSlot = save.GetComponent<SaveSlot>();
                saveSlot.PlayerID = gameData.Key;
                saveSlot.PlayerName = gameData.Value.playerName;
                saveSlot.PlayerNameText.text = gameData.Value.playerName;
                saveSlot.TimestampText.text = gameData.Value.lastUpdated.ToString("M/d/yyyy h:mm tt");
            }
        }
        
        private void SaveLoadMessageHandler(MessageSystem.IMessageEnvelope message)
        {
            if(!message.Message<SaveLoadMessage>().HasValue) return;
            var data = message.Message<SaveLoadMessage>().Value;
            if (data.SaveOperationType == SaveOperation.Delete)
            {
                ClearAndSpawnLoadGamePanels();
            }
        }
    }
}
