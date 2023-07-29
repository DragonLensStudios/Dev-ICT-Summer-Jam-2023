using System;
using System.Collections.Generic;
using System.IO;
using DLS.Achievements;
using DLS.Game.Managers;
using DLS.Game.PLayers;
using DLS.Game.Utilities;
using DLS.Game.Utilities.Json;
using Newtonsoft.Json;
using UnityEngine;

namespace DLS.Core.Data_Persistence
{
    /// <summary>  
    /// Handles saving, loading, and managing save data stored in files.
    /// </summary>
    public class FileDataHandler
    {
        /// <summary>
        /// Full path to the data directory where profiles are saved. 
        /// </summary>
        private string dataDirPath = "";
        
        /// <summary>
        /// Name of the data file for each profile.
        /// </summary>
        private string dataFileName = "";
        
        /// <summary>
        /// Whether to encrypt/decrypt data when reading/writing.
        /// </summary>
        private bool useEncryption = false;
        
        /// <summary>
        /// Encryption key used if encryption is enabled.
        /// Should be kept private.
        /// </summary>
        private readonly string encryptionCodeWord = "53cur3YK37W0rd";
        
        /// <summary>
        /// File extension used for backup files.
        /// </summary>
        private readonly string backupExtension = ".bak";

        /// <summary>
        /// Constructs a new FileDataHandler.
        /// </summary>
        /// <param name="dataDirPath">Path to directory where profile data is stored.</param>
        /// <param name="dataFileName">Name of data file.</param>
        /// <param name="useEncryption">Whether to encrypt/decrypt data.</param>
        public FileDataHandler(string dataDirPath, string dataFileName, bool useEncryption) 
        {
            this.dataDirPath = dataDirPath;
            this.dataFileName = dataFileName;
            this.useEncryption = useEncryption;
        }

        /// <summary>
        /// Loads the game data for the specified profile.
        /// </summary>
        /// <param name="playerID">The profile ID to load.</param>
        /// <param name="allowRestoreFromBackup">Whether to attempt restore from backup if load fails.</param>
        /// <returns>The loaded game data, or null if not found.</returns>
        public GameData Load(SerializableGuid playerID, string playerName, bool allowRestoreFromBackup = true) 
        {
            // base case - if the profileId is null, return right away
            if (playerID.Guid == Guid.Empty) 
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(playerName))
            {
                return null;
            }

            // use Path.Combine to account for different OS's having different path separators
            string fullPath = Path.Combine(dataDirPath, playerName + ";" + playerID.Guid, dataFileName);
            GameData loadedData = null;
            if (File.Exists(fullPath))
            {
                try 
                {
                    // load the serialized data from the file
                    string dataToLoad = "";
                    using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            dataToLoad = reader.ReadToEnd();
                        }
                    }

                    // optionally decrypt the data
                    if (useEncryption) 
                    {
                        dataToLoad = EncryptDecrypt(dataToLoad);
                    }

                    // deserialize the data from Json back into the C# object
                    //loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
                    loadedData = JsonConvert.DeserializeObject<GameData>(dataToLoad, JsonUtil.JsonSettingsForUnityObjects());
                }
                catch (Exception e) 
                {
                    // since we're calling Load(..) recursively, we need to account for the case where
                    // the rollback succeeds, but data is still failing to load for some other reason,
                    // which without this check may cause an infinite recursion loop.
                    if (allowRestoreFromBackup) 
                    {
                        Debug.LogWarning($"Failed to load data file. Attempting to roll back.\n{e.Message}");
                        bool rollbackSuccess = AttemptRollback(fullPath);
                        if (rollbackSuccess)
                        {
                            // try to load again recursively
                            loadedData = Load(playerID, playerName, false);
                        }
                    }
                    // if we hit this else block, one possibility is that the backup file is also corrupt
                    else 
                    {
                        Debug.LogError($"Error occured when trying to load file at path: {fullPath} and backup did not work.\n {e.Message}");
                    }
                }
            }
            return loadedData;
        }

        /// <summary>
        /// Saves the given game data to file for the specified profile.
        /// </summary>
        /// <param name="data">The game data to save.</param>
        /// <param name="playerID">The profile ID to save the data for.</param>
        public void Save(GameData data, SerializableGuid playerID, string playerName) 
        {
            // base case - if the profileId is null, return right away
            if (playerID.Guid == Guid.Empty) 
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(playerName))
            {
                return;;
            }

            // use Path.Combine to account for different OS's having different path separators
            string fullPath = Path.Combine(dataDirPath, playerName + ";" + playerID.Guid, dataFileName);
            string backupFilePath = fullPath + backupExtension;
            try 
            {
                // create the directory the file will be written to if it doesn't already exist
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

                // serialize the C# game data object into Json
                //string dataToStore = JsonUtility.ToJson(data, true);
                string dataToStore = JsonConvert.SerializeObject(data, Formatting.Indented, JsonUtil.JsonSettingsForUnityObjects());

                // optionally encrypt the data
                if (useEncryption) 
                {
                    dataToStore = EncryptDecrypt(dataToStore);
                }

                // write the serialized data to the file
                using (FileStream stream = new FileStream(fullPath, FileMode.Create))
                {
                    using (StreamWriter writer = new StreamWriter(stream)) 
                    {
                        writer.Write(dataToStore);
                    }
                }

                // verify the newly saved file can be loaded successfully
                GameData verifiedGameData = Load(playerID, playerName);
                // if the data can be verified, back it up
                if (verifiedGameData != null) 
                {
                    File.Copy(fullPath, backupFilePath, true);
                }
                // otherwise, something went wrong and we should throw an exception
                else 
                {
                    throw new Exception("Save file could not be verified and backup could not be created.");
                }

            }
            catch (Exception e) 
            {
                Debug.LogError($"Error occured when trying to save data to file: {fullPath}\n{e.Message}");
            }
        }

        /// <summary>
        /// Deletes the saved data for the specified profile.
        /// </summary>
        /// <param name="playerID">The profile ID to delete.</param>
        public void Delete(SerializableGuid playerID, string playerName) 
        {
            // base case - if the profileId is null, return right away
            if (playerID.Guid == Guid.Empty) 
            {
                return;
            }

            string fullPath = Path.Combine(dataDirPath, playerName + ";" + playerID.Guid, dataFileName);
            try 
            {
                // ensure the data file exists at this path before deleting the directory
                if (File.Exists(fullPath)) 
                {
                    // delete the profile folder and everything within it
                    Directory.Delete(Path.GetDirectoryName(fullPath), true);
                }
                else 
                {
                    Debug.LogWarning($"Tried to delete profile data, but data was not found at path: {fullPath}");
                }
            }
            catch (Exception e) 
            {
                Debug.LogError($"Failed to delete profile data for profileId: {playerID} at path: {fullPath}\n{e.Message}");
            }
        }
        
        /// <summary>
        /// Loads the game data for all existing profiles.
        /// </summary>
        /// <returns>A dictionary mapping profile IDs to their game data.</returns>
        public Dictionary<SerializableGuid, GameData> LoadAllProfiles() 
        {
            Dictionary<SerializableGuid, GameData> profileDictionary = new Dictionary<SerializableGuid, GameData>();

            // loop over all directory names in the data directory path
            IEnumerable<DirectoryInfo> dirInfos = new DirectoryInfo(dataDirPath).EnumerateDirectories();
            foreach (DirectoryInfo dirInfo in dirInfos)
            {
                var fileName = dirInfo.Name.Split(';');
                string playerName = fileName[0];
                SerializableGuid playerID = new(Guid.Parse(fileName[1]));
                 

                // defensive programming - check if the data file exists
                // if it doesn't, then this folder isn't a profile and should be skipped
                string fullPath = Path.Combine(dataDirPath, playerName + ";" + playerID.Guid, dataFileName);
                if (!File.Exists(fullPath))
                {
                    Debug.LogWarning($"Skipping directory when loading all profiles because it does not contain data: {playerID}");
                    continue;
                }

                // load the game data for this profile and put it in the dictionary
                GameData profileData = Load(playerID, playerName);
                // defensive programming - ensure the profile data isn't null,
                // because if it is then something went wrong and we should let ourselves know
                if (profileData != null) 
                {
                    if (!profileDictionary.TryAdd(playerID, profileData))
                    {
                         Debug.LogError("Duplicate Profile ID when loading");                       
                    }
                }
                else 
                {
                    Debug.LogError($"Tried to load profile but something went wrong. ProfileId: {playerID}");
                }
            }

            return profileDictionary;
        }

        /// <summary>
        /// Gets the profile ID of the most recently updated profile.
        /// </summary>
        /// <returns>The profile ID of the most recently updated data.</returns>
        public (SerializableGuid playerID, string playerName, Vector3 playerPosition, SerializableGuid levelID, string levelName, List<PlayerAchievementProgress> playerAchievements, List<SavedGameObject> savedGameObjects) GetMostRecentlyUpdatedPlayer() 
        {
            SerializableGuid mostRecentPlayerID = new(Guid.Empty);
            string mostRecentPlayerName = string.Empty;
            Vector3 mostRecentPlayerPosition = Vector3.zero;
            SerializableGuid mostRecentPlayerLevelID = new SerializableGuid(Guid.Empty);
            string mostRecentPlayerLevelName = string.Empty;
            List<PlayerAchievementProgress> mostRecentPlayerAchievements = new();
            List<SavedGameObject> mostRecentPlayerSavedGameObjects = new();
            Dictionary<SerializableGuid, GameData> profilesGameData = LoadAllProfiles();
            foreach (KeyValuePair<SerializableGuid, GameData> pair in profilesGameData) 
            {
                SerializableGuid playerID = pair.Key;
                GameData gameData = pair.Value;

                // skip this entry if the gamedata is null
                if (gameData == null) 
                {
                    continue;
                }
                
                // if this is the first data we've come across that exists, it's the most recent so far
                if (mostRecentPlayerID.Guid == Guid.Empty) 
                {
                    mostRecentPlayerID = playerID;
                    mostRecentPlayerName = gameData.playerName;
                    mostRecentPlayerPosition = gameData.playerPosition;
                    mostRecentPlayerLevelID = gameData.currentLevelID;
                    mostRecentPlayerLevelName = gameData.currentLevelName;
                    mostRecentPlayerAchievements = gameData.playerAchievements;
                    mostRecentPlayerSavedGameObjects = gameData.savedGameObjects;
                }
                // otherwise, compare to see which date is the most recent
                else 
                {
                    DateTime mostRecentDateTime = profilesGameData[mostRecentPlayerID].lastUpdated;
                    DateTime newDateTime = gameData.lastUpdated;
                    // the greatest DateTime value is the most recent
                    if (newDateTime > mostRecentDateTime) 
                    {
                        mostRecentPlayerID = playerID;
                        mostRecentPlayerName = gameData.playerName;
                        mostRecentPlayerPosition = gameData.playerPosition;
                        mostRecentPlayerLevelID = gameData.currentLevelID;
                        mostRecentPlayerLevelName = gameData.currentLevelName;
                        mostRecentPlayerAchievements = gameData.playerAchievements;
                        mostRecentPlayerSavedGameObjects = gameData.savedGameObjects;
                    }
                }
            }
            return (mostRecentPlayerID, mostRecentPlayerName, mostRecentPlayerPosition, mostRecentPlayerLevelID, mostRecentPlayerLevelName,mostRecentPlayerAchievements, mostRecentPlayerSavedGameObjects);
        }

        /// <summary>
        /// Encrypts/Decrypts the given data using a simple XOR cipher. 
        /// </summary>
        /// <param name="data">The data to encrypt/decrypt.</param>
        /// <returns>The encrypted/decrypted version of the data.</returns>
        private string EncryptDecrypt(string data) 
        {
            string modifiedData = "";
            for (int i = 0; i < data.Length; i++) 
            {
                modifiedData += (char) (data[i] ^ encryptionCodeWord[i % encryptionCodeWord.Length]);
            }
            return modifiedData;
        }

        /// <summary>
        /// Attempts to roll back to the backup file if it exists.
        /// </summary>
        /// <param name="fullPath">Path to main data file.</param>
        /// <returns>True if successfully rolled back, otherwise false.</returns>
        private bool AttemptRollback(string fullPath) 
        {
            bool success = false;
            string backupFilePath = fullPath + backupExtension;
            try 
            {
                // if the file exists, attempt to roll back to it by overwriting the original file
                if (File.Exists(backupFilePath))
                {
                    File.Copy(backupFilePath, fullPath, true);
                    success = true;
                    Debug.LogWarning($"Had to roll back to backup file at: {backupFilePath}");
                }
                // otherwise, we don't yet have a backup file - so there's nothing to roll back to
                else 
                {
                    throw new Exception("Tried to roll back, but no backup file exists to roll back to.");
                }
            }
            catch (Exception e) 
            {
                Debug.LogError($"Error occured when trying to roll back to backup file at: {backupFilePath}\n {e.Message}");
            }

            return success;
        }
    }
}
