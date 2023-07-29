using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DLS.Game.Enums;
using DLS.Game.GameStates;
using DLS.Game.Managers;
using DLS.Game.Messages;
using DLS.Game.PLayers;
using DLS.Game.Utilities;
using DLS.Utilities;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DLS.Core.Data_Persistence
{
    public class DataPersistenceManager : MonoBehaviour
    {
        public static DataPersistenceManager instance { get; private set; }

        [Header("Debugging")]
        [SerializeField] private bool disableDataPersistence = false;
        [SerializeField] private bool initializeDataIfNull = false;
        [SerializeField] private bool overrideSelectedProfileId = false;
        [SerializeField] private SerializableGuid testSelectedPlayerID = new(Guid.Empty);

        [Header("File Storage Config")]
        [SerializeField] private string fileName;
        [SerializeField] private bool useEncryption;

        [Header("Auto Saving Configuration")]
        [SerializeField] private bool useAutoSave = false;
        [SerializeField] private bool displayAutoSaveNotification = false;
        [SerializeField] private float displayAutoSaveNotificationTime = 2.5f;
        [SerializeField] private bool useSaveOnExit = false;
        [SerializeField] private float autoSaveTimeSeconds = 60f;

        [SerializeField]
        private SerializableGuid selectedPlayerID = new(Guid.Empty);
        
        [SerializeField]
        private string selectedPlayerName = String.Empty;
        
        [SerializeField]
        private Vector3 selectedPlayerPosition = Vector3.zero;

        [SerializeField]
        private SerializableGuid selectedPlayerLevelID = new SerializableGuid(Guid.Empty);

        [SerializeField]
        private string selectedPlayerLevelName = string.Empty;

        [SerializeField] 
        private List<SavedGameObject> selectedPlayerSavedGameObjects = new();

        public FileDataHandler dataHandler;

        private GameData gameData;
        private List<IDataPersistence> dataPersistenceObjects;

        private Coroutine autoSaveCoroutine;

        private PlayerController player;

        public SerializableGuid SelectedPlayerID { get => selectedPlayerID; set => selectedPlayerID = value; }
        public string SelectedPlayerName { get => selectedPlayerName; set => selectedPlayerName = value; }

        private void Awake() 
        {
            if (instance != null) 
            {
                Debug.Log("Found more than one Data Persistence Manager in the scene. Destroying the newest one.");
                Destroy(gameObject);
                return;
            }
            instance = this;
            DontDestroyOnLoad(gameObject);

            if (disableDataPersistence) 
            {
                Debug.LogWarning("Data Persistence is currently disabled!");
            }

            dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);

            InitializeSelectedPlayerId();
            

        }

        private void OnEnable() 
        {
            MessageSystem.MessageManager.RegisterForChannel<SaveLoadMessage>(MessageChannels.Saves, SaveOrLoadMessageHandler);
        }
        
        private void OnDisable() 
        {
            MessageSystem.MessageManager.UnregisterForChannel<SaveLoadMessage>(MessageChannels.Saves, SaveOrLoadMessageHandler);

        }

        private void Start()
        {
            if (useAutoSave)
            {
                StartCoroutine(AutoSave());
            }
            
            player = PlayerManager.Instance.Player;

        }

        public void DeleteProfileData(SerializableGuid playerID, string playerName) 
        {
            // delete the data for this profile id
            dataHandler.Delete(playerID, playerName);
            // initialize the selected profile id
            InitializeSelectedPlayerId();
            // reload the game so that our data matches the newly selected profile id
            LoadGame(selectedPlayerID, selectedPlayerName);
        }

        private void InitializeSelectedPlayerId() 
        {
            if(player == null) return;;
            var mostRecentPlayer = dataHandler.GetMostRecentlyUpdatedPlayer();
            selectedPlayerID = mostRecentPlayer.playerID;
            selectedPlayerName = mostRecentPlayer.playerName;
            selectedPlayerPosition = mostRecentPlayer.playerPosition;
            selectedPlayerLevelID = mostRecentPlayer.levelID;
            selectedPlayerLevelName = mostRecentPlayer.levelName;
            selectedPlayerSavedGameObjects = mostRecentPlayer.savedGameObjects;
            
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Player, new PlayerDataMessage(selectedPlayerID, player.ObjectName,player.transform.position, player.MoveSpeed, player.CurrentLevelID, player.CurrentLevelName, player.AchievementProgressList, player.SavedGameObjects));
            if (overrideSelectedProfileId) 
            {
                selectedPlayerID = testSelectedPlayerID;
                Debug.LogWarning("Overrode selected profile id with test id: " + testSelectedPlayerID);
            }

            if (selectedPlayerID.Guid == Guid.Empty)
            {
                selectedPlayerID = player.ID;
            }
        }

        public void NewGame() 
        {
            gameData = new GameData();
            if (selectedPlayerID.Guid == Guid.Empty)
            {
                if (player != null)
                {
                    selectedPlayerID = player.ID;
                    selectedPlayerName = player.ObjectName;
                }
            }
            
            SaveGame(selectedPlayerID, selectedPlayerName);
        }

        public void LoadGame(SerializableGuid playerID, string playerName)
        {
            // return right away if data persistence is disabled
            if (disableDataPersistence) 
            {
                return;
            }

            // load any saved data from a file using the data handler
            gameData = dataHandler.Load(playerID, playerName);

            // start a new game if the data is null and we're configured to initialize data for debugging purposes
            if (gameData == null && initializeDataIfNull) 
            {
                NewGame();
            }

            // if no data can be loaded, don't continue
            if (gameData == null) 
            {
                Debug.Log("No data was found. A New Game needs to be started before data can be loaded.");
                return;
            }

            dataPersistenceObjects = FindAllDataPersistenceObjects();
            // push the loaded data to all other scripts that need it
            foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
            {
                dataPersistenceObj.LoadData(gameData);
            }
        }

        public void SaveGame(SerializableGuid playerID, string playerName)
        {
            // return right away if data persistence is disabled
            if (disableDataPersistence) 
            {
                return;
            }

            // if we don't have any data to save, log a warning here
            if (gameData == null) 
            {
                NewGame();
                Debug.Log("No data was found. Starting a New Game.");
                return;
            }
            dataPersistenceObjects = FindAllDataPersistenceObjects();
            // pass the data to other scripts so they can update it
            foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects) 
            {
                dataPersistenceObj.SaveData(gameData);
            }

            // timestamp the data so we know when it was last saved
            gameData.lastUpdated = DateTime.Now;

            // save that data to a file using the data handler
            dataHandler.Save(gameData, playerID, playerName);
        }

        private void OnApplicationQuit() 
        {
            if (useSaveOnExit)
            {
                SaveGame(selectedPlayerID, selectedPlayerName);
            }
        }

        private List<IDataPersistence> FindAllDataPersistenceObjects() 
        {
            IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None).OfType<IDataPersistence>();

            return new List<IDataPersistence>(dataPersistenceObjects);
        }

        public bool HasGameData() 
        {
            return GetAllProfilesGameData().Count > 0;
        }

        public Dictionary<SerializableGuid, GameData> GetAllProfilesGameData() 
        {
            return dataHandler.LoadAllProfiles();
        }

        private IEnumerator AutoSave() 
        {
            while (useAutoSave)
            {
                if (GameManager.Instance.IsCurrentState<GamePlayingState>())
                {
                    yield return new WaitForSeconds(autoSaveTimeSeconds);
                    SaveGame(player.ID, player.ObjectName);
                    if (displayAutoSaveNotification)
                    {
                        MessageSystem.MessageManager.SendImmediate(MessageChannels.UI, new PopupMessage("Saving...", PopupType.Notification, PopupPosition.Bottom, displayAutoSaveNotificationTime));
                    }
                }
                yield return new WaitForSeconds(1f); // Wait for 1 second before the next iteration
            }
        }


        private void SaveOrLoadMessageHandler(MessageSystem.IMessageEnvelope message)
        {
            if(!message.Message<SaveLoadMessage>().HasValue) return;
            var data = message.Message<SaveLoadMessage>().Value;

            switch (data.SaveOperationType)
            {
                case SaveOperation.Save:
                    SaveGame(data.PlayerID, data.PlayerName);
                    break;
                case SaveOperation.Load:
                    LoadGame(data.PlayerID, data.PlayerName);
                    break;
                case SaveOperation.Delete:
                    DeleteProfileData(data.PlayerID, data.PlayerName);
                    break;
            }
        }
        
    }
}
