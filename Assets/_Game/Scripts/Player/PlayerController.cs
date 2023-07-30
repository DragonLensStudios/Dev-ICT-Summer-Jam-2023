using System;
using System.Collections.Generic;
using System.Linq;
using DLS.Achievements;
using DLS.Core;
using DLS.Core.Data_Persistence;
using DLS.Core.Input;
using DLS.Game.Enums;
using DLS.Game.GameStates;
using DLS.Game.Interfaces;
using DLS.Game.Levels;
using DLS.Game.Managers;
using DLS.Game.Messages;
using DLS.Game.Utilities;
using DLS.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DLS.Game.PLayers
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
    public class PlayerController: ActorController, IDataPersistence, IHealth
    {
        [field: SerializeField] public int CurrentHealth { get; set; }
        [field: SerializeField] public int MaxHealth { get; set; }
        [field: SerializeField] public int Resources { get; set; } = 5;
        [field: SerializeField] public bool IsAlive { get; set; }
        
        [field: SerializeField] public float MoveSpeed { get; set; }
        [field: SerializeField] public string CurrentLevelName { get; set; }
        
        [field: SerializeField] public SerializableGuid CurrentLevelID { get; set; }
        [field: SerializeField] public AchievementManagerSettings AchievementManager { get; set; }
        [field: SerializeField] public List<PlayerAchievementProgress> AchievementProgressList { get; set; }
        [field: SerializeField] public List<SavedGameObject> SavedGameObjects { get; set; }
        [field: SerializeField] public Vector2 lastMovementPosition { get; set; }
        
        private Vector2 movement;
        private Rigidbody2D rb;
        private Animator anim;
        private PlayerInputActions playerInput;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            anim = GetComponent<Animator>();
            playerInput = new PlayerInputActions();
            id = new(Guid.NewGuid());
        }

        private void Start()
        {
            SetAchievementProgress();
        }

        public void SetAchievementProgress()
        {
            AchievementProgressList.Clear();
            // Create a dictionary from AchievementProgressList for easy lookup
            var progressDictionary = AchievementProgressList.ToDictionary(x => x.AchievementKey, x => x);

            // Iterate through AchievementManager.AchievementList and only add new Achievements
            foreach (var achievement in AchievementManager.AchievementList)
            {
                if (!progressDictionary.ContainsKey(achievement.Key))
                {
                    AchievementProgressList.Add(new PlayerAchievementProgress(achievement.Key));
                }
            }

            // You may want to order the list as per some property
            AchievementProgressList = AchievementProgressList.OrderBy(x => x.AchievementKey).ToList();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            playerInput.Enable();
            playerInput.Player.Move.performed += MoveOnperformed;
            playerInput.Player.Move.canceled += MoveOncanceled;
            playerInput.Player.Interact.performed += InteractOnperformed;
            MessageSystem.MessageManager.RegisterForChannel<PauseMessage>(MessageChannels.GameFlow, HandlePause);
            MessageSystem.MessageManager.RegisterForChannel<LevelMessage>(MessageChannels.Level, HandleLevelChange);
        }
        
        protected override void OnDisable()
        {
            base.OnDisable();
            playerInput.Disable();
            playerInput.Player.Move.performed -= MoveOnperformed;
            playerInput.Player.Move.canceled -= MoveOncanceled;
            playerInput.Player.Interact.performed -= InteractOnperformed;
            MessageSystem.MessageManager.UnregisterForChannel<PauseMessage>(MessageChannels.GameFlow, HandlePause);
            MessageSystem.MessageManager.UnregisterForChannel<LevelMessage>(MessageChannels.Level, HandleLevelChange);

        }
        
        private void MoveOncanceled(InputAction.CallbackContext input)
        {
            lastMovementPosition = movement;
            movement = Vector2.zero;
            anim.SetBool("isMoving", false);
            anim.SetFloat("move_x", lastMovementPosition.x);
            anim.SetFloat("move_y", lastMovementPosition.y);
        }

        private void MoveOnperformed(InputAction.CallbackContext input)
        {
            movement = input.ReadValue<Vector2>();
            anim.SetBool("isMoving", true);
            anim.SetFloat("move_x", movement.x);
            anim.SetFloat("move_y", movement.y);
        }
        
        private void InteractOnperformed(InputAction.CallbackContext input)
        {
            Interact();
        }
        
        private void FixedUpdate() => rb.position += movement * (MoveSpeed * Time.fixedDeltaTime);
        
        private void HandlePause(MessageSystem.IMessageEnvelope message)
        {
            if(!message.Message<PauseMessage>().HasValue) return;
            if (message.Message<PauseMessage>().Value.IsPaused)
            {
                playerInput.Disable();
                isMovementDisabled = true;
            }
            else
            {
                playerInput.Enable();
                isMovementDisabled = false;
            }
        }
        
        private void HandleLevelChange(MessageSystem.IMessageEnvelope message)
        {
            if(!message.Message<LevelMessage>().HasValue) return;
            var data = message.Message<LevelMessage>().Value;
            if (data.LevelState != LevelState.Loading) return;
            CurrentLevelName = data.LevelName;
            CurrentLevelID = data.LevelID;
            transform.position = data.Position;
        }

        public void LoadData(GameData data)
        {
            id = data.playerID;
            objectName = data.playerName;
            transform.position = data.playerPosition;
            MoveSpeed = data.playerMoveSpeed;
            CurrentLevelName = data.currentLevelName;
            CurrentLevelID = data.currentLevelID;
            AchievementProgressList = data.playerAchievements;
            SavedGameObjects = data.savedGameObjects;
            lastMovementPosition = data.lastMovementPosition;
            anim.SetFloat("move_x", lastMovementPosition.x);
            anim.SetFloat("move_y", lastMovementPosition.y);
        }

        public void SaveData(GameData data)
        {
            data.playerID = id;
            data.playerName = objectName;
            data.playerPosition = transform.position;
            data.playerMoveSpeed = MoveSpeed;
            data.currentLevelName = CurrentLevelName;
            data.currentLevelID = CurrentLevelID;
            data.playerAchievements = AchievementProgressList;
            data.savedGameObjects = SavedGameObjects;
            data.lastMovementPosition = lastMovementPosition;
        }
        
        public void SetHealth(int amount)
        {
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Player, new HealthChangedMessage(id, amount, HealthChangeType.Set));
            CurrentHealth = amount > MaxHealth ? MaxHealth : amount;

            if (CurrentHealth <= 0)
            {
                Die();
            }

        }

        public void AddHealth(int amount)
        {
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Player, new HealthChangedMessage(id, amount, HealthChangeType.Add));
            if (CurrentHealth + amount > MaxHealth)
            {
                CurrentHealth = MaxHealth;
            }
            else
            {
                CurrentHealth += amount;
            }
        }

        public void RemoveHealth(int amount)
        {
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Player, new HealthChangedMessage(id, amount, HealthChangeType.Remove));
            if (CurrentHealth - amount <= 0)
            {
                CurrentHealth = 0;
                Die();
            }
            else
            {
                CurrentHealth -= amount;
            }
        }

        public void Die()
        {
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Player, new DeathMessage(id));
        }
    }
}