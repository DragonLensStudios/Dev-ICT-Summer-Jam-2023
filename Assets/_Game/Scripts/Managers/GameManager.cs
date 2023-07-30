using System;
using System.Collections.Generic;
using System.Linq;
using DLS.Core.Input;
using DLS.Game.Enums;
using DLS.Game.GameStates;
using DLS.Game.Messages;
using DLS.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DLS.Game.Managers
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager instance;
        public static GameManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindFirstObjectByType<GameManager>(FindObjectsInactive.Include);

                    if (instance == null)
                    {
                        GameObject managerObject = new GameObject("Game Manager");
                        instance = managerObject.AddComponent<GameManager>();
                    }
                }

                return instance;
            }
        }

        [SerializeField]
        public GameState CurrentState;

        [SerializeField]
        public GameState InitialState;

        [SerializeField] 
        public List<GameState> AllStates;

        private PlayerInputActions playerInput;

        private void OnEnable()
        {
            playerInput.Enable();
            playerInput.Player.Pause.performed += PauseOnperformed;
            MessageSystem.MessageManager.RegisterForChannel<GameStateMessage>(MessageChannels.GameFlow, ChangeStateHandler);
            MessageSystem.MessageManager.RegisterForChannel<PauseMessage>(MessageChannels.GameFlow, PausedMessageHandler);
            MessageSystem.MessageManager.RegisterForChannel<BuilderEndMessage>(MessageChannels.Builder, BuilderEndMessageHandler);
            MessageSystem.MessageManager.RegisterForChannel<DeathMessage>(MessageChannels.Player, DeathMessageHandler);
        }

        private void DeathMessageHandler(MessageSystem.IMessageEnvelope message)
        {
            if(!message.Message<DeathMessage>().HasValue) return;;
            var data = message.Message<DeathMessage>().Value;
            if(data.ObjectID.Equals(PlayerManager.Instance.Player.ID))
            {
                SetState(GetStateByType<GameOverState>());
            }
        }

        private void OnDisable()
        {
            playerInput.Disable();
            playerInput.Player.Pause.performed -= PauseOnperformed;

            MessageSystem.MessageManager.UnregisterForChannel<GameStateMessage>(MessageChannels.GameFlow, ChangeStateHandler);
            MessageSystem.MessageManager.UnregisterForChannel<PauseMessage>(MessageChannels.GameFlow, PausedMessageHandler);
            MessageSystem.MessageManager.UnregisterForChannel<BuilderEndMessage>(MessageChannels.Builder, BuilderEndMessageHandler);
            MessageSystem.MessageManager.UnregisterForChannel<DeathMessage>(MessageChannels.Player, DeathMessageHandler);


        }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

            if (AllStates.Count <= 0)
            {
                AllStates = Resources.LoadAll<GameState>("States").ToList();
            }

            playerInput = new PlayerInputActions();
        }

        private void Start()
        {
            if (InitialState == null) return;
            SetState(InitialState, true);
        }

        private void Update()
        {
            if (CurrentState != null)
                CurrentState.Update();
        }

        public void SetState(GameState newState, bool isStartingGame = false)
        {
            if (CurrentState != null)
            {
                CurrentState.Exit();
            }

            if (CurrentState == newState)
            {
                return;
            }
    
            CurrentState = newState;

            if (CurrentState != null && (!isStartingGame || (isStartingGame && CurrentState == InitialState)))
            {
                CurrentState.Enter();
            }
        }

        public bool IsCurrentState<T>() where T : GameState
        {
            return CurrentState is T;
        }

        public GameState GetStateByType<T>() where T : GameState
        {
            return AllStates.FirstOrDefault(x => x.GetType() == typeof(T));
        }    
        private void ChangeStateHandler(MessageSystem.IMessageEnvelope message)
        {
            if(!message.Message<GameStateMessage>().HasValue) return;
            var data = message.Message<GameStateMessage>().Value;
            SetState(data.State);
        }
        
        private void PausedMessageHandler(MessageSystem.IMessageEnvelope message)
        {
            if(!message.Message<PauseMessage>().HasValue) return;
            var data = message.Message<PauseMessage>().Value;
            var gameplayState = GetStateByType<GamePlayingState>();
            if (IsCurrentState<PausedState>())
            {
                if (!data.IsPaused)
                {
                    if (gameplayState != null)
                    {
                        SetState(gameplayState);
                    }
                }
            }
            if(!data.ShowPauseMenu) return;
            if(IsCurrentState<MainMenuState>()) return;
            if (!IsCurrentState<GamePlayingState>()) return;
            var pausedState =  GetStateByType<PausedState>();
            if (pausedState != null)
            {
                SetState(pausedState);                
            }

        }
        
        private void PauseOnperformed(InputAction.CallbackContext input)
        {
            if (!IsCurrentState<PausedState>() && !IsCurrentState<MainMenuState>())
            {
                MessageSystem.MessageManager.SendImmediate(MessageChannels.GameFlow, new PauseMessage(true, true));
            }
            else if (IsCurrentState<PausedState>())
            {
                MessageSystem.MessageManager.SendImmediate(MessageChannels.GameFlow, new PauseMessage(false));
            }
        }

        private void BuilderEndMessageHandler(MessageSystem.IMessageEnvelope message)
        {
            if (!message.Message<BuilderEndMessage>().HasValue) return;
            SetState(GetStateByType<EnemyWaveState>());
        }
    }
}
