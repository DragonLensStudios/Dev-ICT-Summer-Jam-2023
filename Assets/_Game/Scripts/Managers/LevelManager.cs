using System;
using System.Collections.Generic;
using System.Linq;
using DLS.Game.Enums;
using DLS.Game.Levels;
using DLS.Game.Messages;
using DLS.Game.PLayers;
using DLS.Utilities;
using UnityEngine;
using UnityEngine.Rendering;

namespace DLS.Game.Managers
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance { get; private set; }

        [field: SerializeField] public List<Level> Levels { get; set; }
        [field: SerializeField] public Level CurrentLevel { get; set; }
        [field: SerializeField] public bool SaveOnLevelChange { get; set; } = true;

        private void OnEnable()
        {
            MessageSystem.MessageManager.RegisterForChannel<LevelMessage>(MessageChannels.Level, ChangeLevelHandler);
        }

        private void OnDisable()
        {
            MessageSystem.MessageManager.UnregisterForChannel<LevelMessage>(MessageChannels.Level, ChangeLevelHandler);
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
        }

        private void Start()
        {
            Levels = Resources.LoadAll<Level>("Levels").ToList();
        }

        public Level ChangeLevel(Level level)
        {
            if (CurrentLevel != null && CurrentLevel.LevelState == LevelState.Loaded)
            {
                CurrentLevel.Exit();
            }
            if (CurrentLevel != level && level.LevelState == LevelState.Unloaded)
            {
                CurrentLevel = level;
                CurrentLevel.Enter();
            }

            return CurrentLevel;
        }
        
        private void ChangeLevelHandler(MessageSystem.IMessageEnvelope message)
        {
            if(!message.Message<LevelMessage>().HasValue) return;;
            var data = message.Message<LevelMessage>().Value;
            var level = Levels.FirstOrDefault(x => x.ID.Equals(data.LevelID));
            if (data.LevelState == LevelState.Unloading)
            {
                level.Exit();
                CurrentLevel = null;
                return;
            }
            if (SaveOnLevelChange)
            {
                MessageSystem.MessageManager.SendImmediate(MessageChannels.Saves, new SaveLoadMessage(PlayerManager.Instance.Player.ID, PlayerManager.Instance.Player.ObjectName, SaveOperation.Save));
            }
            if (level != null && level.LevelState == LevelState.Unloaded)
            {
                ChangeLevel(level);
            }


        }

        private void OnApplicationQuit()
        {
            for (int i = 0; i < Levels.Count; i++)
            {
                Levels[i].LevelState = LevelState.Unloaded;
            }
        }
    }
}