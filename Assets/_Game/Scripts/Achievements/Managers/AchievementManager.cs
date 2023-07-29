using System;
using System.Collections.Generic;
using System.Linq;
using DLS.Core;
using DLS.Core.Data_Persistence;
using DLS.Game.Enums;
using DLS.Game.Managers;
using DLS.Game.Messages;
using DLS.Game.PLayers;
using DLS.Game.Utilities;
using DLS.Utilities;
using UnityEngine;

namespace DLS.Achievements
{
    /// <summary>
    /// Controls interactions with the Achievement System
    /// </summary>
    [System.Serializable]
    public class AchievementManager : MonoBehaviour, IDataPersistence
    {
        public static AchievementManager Instance = null;
        
        [field: SerializeField] public SerializableGuid LevelID { get; set; }
        [field: SerializeField] public SerializableGuid PrefabID { get; set; }
        [field: SerializeField] public SerializableGuid ID { get; set; }
        [field: SerializeField] public string ObjectName { get; set; }
        [field: SerializeField] public List<PlayerAchievementProgress> PlayerAchievementProgress { get; set; }
        [field: SerializeField] public AchievementManagerSettings Manager { get; set; }
        [field: SerializeField] public AchievementStack AchievementStack { get; set; }
        
        private PlayerController player;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
            DontDestroyOnLoad(gameObject);
            AchievementStack = GetComponentInChildren<AchievementStack>();
            LoadAchievements();
        }

        private void Start()
        {
            player = PlayerManager.Instance.Player;
            if(player == null) return;
            PlayerAchievementProgress = player.AchievementProgressList;
        }

        private void OnEnable()
        {
            MessageSystem.MessageManager.RegisterForChannel<AchievementMessage>(MessageChannels.Achievement, HandleAchievementMessage);
        }

        private void OnDisable()
        {
            MessageSystem.MessageManager.UnregisterForChannel<AchievementMessage>(MessageChannels.Achievement, HandleAchievementMessage);
        }
        
        /// <summary>
        /// Display achievements progress to screen  
        /// </summary>
        /// <param name="index">Index of achievement to display</param>
        private void DisplayUnlock(int index)
        {
            var achievement = Manager.AchievementList[index];
            if(achievement == null) return;;
            if(player == null) return;
            var achievementProgress = player.AchievementProgressList.FirstOrDefault(x => x.AchievementKey.Equals(achievement.Key));
            if(achievementProgress == null) return;
            
            if ((!Manager.DisplayAchievements || achievement.Spoiler) && !achievementProgress.Achieved) return;

            //If not achieved
            if (achievement.Progression && achievementProgress.Progress < achievement.ProgressGoal)
            {
                int Steps = (int)achievement.ProgressGoal / (int)achievement.NotificationFrequency;

                //Loop through all notification point backwards from last possible option
                for (int i = Steps; i > achievementProgress.LastProgressUpdate; i--)
                {
                    //When it finds the largest valid notification point
                    if (achievementProgress.Progress >= achievement.NotificationFrequency * i)
                    {
                        PlaySound(!string.IsNullOrEmpty(achievement.ProgressMadeSound)
                            ? achievement.ProgressMadeSound
                            : Manager.DefaultProgressMadeSound);
                        
                        achievementProgress.LastProgressUpdate = i;
                        AchievementStack.ScheduleAchievementDisplay(index);
                        return;
                    }
                }
            }
            else
            {
                PlaySound(!string.IsNullOrEmpty(achievement.AchievedSound)
                    ? achievement.AchievedSound
                    : Manager.DefaultAchievedSound);
                AchievementStack.ScheduleAchievementDisplay(index);
            }
        }
        
#region Unlock and Progress
        /// <summary>
        /// Fully unlocks a progression or goal achievement.
        /// </summary>
        /// <param name="key">The Key of the achievement to be unlocked</param>
        public void Unlock(string key)
        {
            Unlock(FindAchievementIndex(key));
        }
        /// <summary>
        /// Fully unlocks a progression or goal achievement.
        /// </summary>
        /// <param name="index">The index of the achievement to be unlocked</param>
        public void Unlock(int index)
        {
            var achievement = Manager.AchievementList[index];
            if (achievement == null) return;
            if(player == null) return;
            var achievementProgress = player.AchievementProgressList.FirstOrDefault(x => x.AchievementKey.Equals(achievement.Key));
            if(achievementProgress == null) return;
            if (achievementProgress.Achieved) return;
            achievementProgress.Progress = achievement.ProgressGoal;
            achievementProgress.Achieved = true;
            DisplayUnlock(index);

            if (!Manager.UseFinalAchievement) return;
            var allCompleted = player.AchievementProgressList.All(x => !x.AchievementKey.Equals(Manager.FinalAchievementKey) && x.Achieved);
            if (allCompleted)
            {
                Unlock(Manager.FinalAchievementKey);
            }
            
        }
        
        public void Lock(string key)
        {
            Lock(FindAchievementIndex(key));
        }
        
        public void Lock(int index)
        {
            var achievement = Manager.AchievementList[index];
            if (achievement == null) return;
            if(player == null) return;
            var achievementProgress = player.AchievementProgressList.FirstOrDefault(x => x.AchievementKey.Equals(achievement.Key));
            if(achievementProgress is not { Achieved: true }) return;
            achievementProgress.Progress = 0;
            achievementProgress.Achieved = false;
        }
        /// <summary>
        /// Set the progress of an achievement to a specific value. 
        /// </summary>
        /// <param name="Key">The Key of the achievement</param>
        /// <param name="progress">Set progress to this value</param>
        public void SetAchievementProgress(string Key, float progress)
        {
            SetAchievementProgress(FindAchievementIndex(Key), progress);
        }
        /// <summary>
        /// Set the progress of an achievement to a specific value. 
        /// </summary>
        /// <param name="index">The index of the achievement</param>
        /// <param name="progress">Set progress to this value</param>
        public void SetAchievementProgress(int index, float progress)
        {
            var achievement = Manager.AchievementList[index];
            if (achievement == null) return;
            if(player == null) return;
            var achievementProgress = player.AchievementProgressList.FirstOrDefault(x => x.AchievementKey.Equals(achievement.Key));

            if (achievementProgress.Progress >= achievement.ProgressGoal)
            {
                Unlock(index);
            }
            else
            {
                achievementProgress.Progress = progress;
                DisplayUnlock(index);

            }
        }
        /// <summary>
        /// Adds the input amount of progress to an achievement. Clamps achievement progress to its max value.
        /// </summary>
        /// <param name="key">The Key of the achievement</param>
        /// <param name="progress">Add this number to progress</param>
        public void AddAchievementProgress(string key, float progress)
        {
            AddAchievementProgress(FindAchievementIndex(key), progress);
        }
        /// <summary>
        /// Adds the input amount of progress to an achievement. Clamps achievement progress to its max value.
        /// </summary>
        /// <param name="index">The index of the achievement</param>
        /// <param name="progress">Add this number to progress</param>
        public void AddAchievementProgress(int index, float progress)
        {
            var achievement = Manager.AchievementList[index];
            if (achievement == null) return;
            if(player == null) return;
            var achievementProgress = player.AchievementProgressList.FirstOrDefault(x => x.AchievementKey.Equals(achievement.Key));
           if(achievementProgress == null) return;
            if (!achievement.Progression) return;
            if (achievementProgress.Progress + progress >= achievement.ProgressGoal)
            {
                Unlock(index);
            }
            else
            {
                achievementProgress.Progress += progress;
                DisplayUnlock(index);
            }
        }
        
        public void SubtractAchievementProgress(string key, float progress)
        {
            SubtractAchievementProgress(FindAchievementIndex(key), progress);
        }
        
        public void SubtractAchievementProgress(int index, float progress)
        {
            var achievement = Manager.AchievementList[index];
            if (achievement == null) return;
            if(player == null) return;
            var achievementProgress = player.AchievementProgressList.FirstOrDefault(x => x.AchievementKey.Equals(achievement.Key));
            achievementProgress.Progress -= progress;
            DisplayUnlock(index);
            if (achievementProgress.Progress - progress >= achievement.ProgressGoal)
            {
                Unlock(index);
            }
        }
#endregion

# region Miscellaneous
        
        /// <summary>
        /// Does an achievement exist in the list
        /// </summary>
        /// <param name="key">The Key of the achievement to test</param>
        /// <returns>true : if exists. false : does not exist</returns>
        public bool AchievementExists(string key)
        {
            return Manager.AchievementList.Exists(x => x.Key.Equals(key));
        }
        
        /// <summary>
        /// Returns the total number of achievements which have been unlocked.
        /// </summary>
        public int GetAchievedCount()
        {
            return player == null ? 0 : player.AchievementProgressList.Count(x => x.Achieved);
        }
        
        /// <summary>
        /// Returns the current percentage of unlocked achievements.
        /// </summary>
        public float GetAchievedPercentage()
        {
            if (player == null) return 0;
            if (player.AchievementProgressList.Count(x => x.Achieved) == 0) return 0;
            return (float)GetAchievedCount() / Manager.AchievementList.Count * 100;
        }
        #endregion

        #region Persistence
        /// <summary>
        /// Loads all progress and achievement states from player prefs. This function is automatically called if the Auto Load setting is set to true.
        /// </summary>
        public void LoadAchievements()
        {
            Manager.AchievementList = Resources.LoadAll<Achievement>("Achievements").ToList();
        }
        /// <summary>
        /// Clears all saved progress and achieved states.
        /// </summary>
        public void ResetAchievement(string key)
        {
            if (player == null) return;
            var achievementProgress = player.AchievementProgressList.FirstOrDefault(x => x.AchievementKey.Equals(key));
            if (achievementProgress == null) return;
            achievementProgress.Progress = 0;
            achievementProgress.LastProgressUpdate = 0;
            achievementProgress.Achieved = false;
        }
    
        public void ResetAllAchievements()
        {
            if (player == null) return;
            var achievements = player.AchievementProgressList;
            for (int i = 0; i < achievements.Count; i++)
            {
                achievements[i].Progress = 0;
                achievements[i].LastProgressUpdate = 0;
                achievements[i].Achieved = false;
            }
        }
        #endregion

        /// <summary>
        /// Find the index of an achievement with a cetain key
        /// </summary>
        /// <param name="Key">Key of achievevment</param>
        public int FindAchievementIndex(string Key)
        {
            return Manager.AchievementList.FindIndex(x => x.Key.Equals(Key));
        }

        public void PlaySound (string soundName)
        {
            if(!string.IsNullOrWhiteSpace(soundName))
            {
                MessageSystem.MessageManager.SendImmediate(MessageChannels.Audio, new AudioMessage(soundName, AudioOperation.Play, VolumeType.SoundEffects));
            }
        }
        
        private void HandleAchievementMessage(MessageSystem.IMessageEnvelope message)
        {
            if(!message.Message<AchievementMessage>().HasValue) return;
            var data = message.Message<AchievementMessage>().Value;
            switch (data.Operator)
            {
                case AchievementOperator.Add:
                    AddAchievementProgress(data.AchievementKey, data.Progress);
                    break;
                case AchievementOperator.Subtract:
                    SubtractAchievementProgress(data.AchievementKey, data.Progress);
                    break;
                case AchievementOperator.Set:
                    SetAchievementProgress(data.AchievementKey, data.Progress);
                    break;
                case AchievementOperator.Unlock:
                    Unlock(data.AchievementKey);
                    break;
                case AchievementOperator.Lock:
                    Lock(data.AchievementKey);
                    break;
            }
        }

        public void LoadData(GameData data)
        {
            PlayerAchievementProgress = data.playerAchievements;
        }

        public void SaveData(GameData data){}
    }
}