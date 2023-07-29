using System.Collections.Generic;
using DLS.Core.Data_Persistence;
using DLS.Game.Enums;
using DLS.Game.GameStates;
using DLS.Game.Managers;
using DLS.Game.Messages;
using DLS.Game.Transition;
using DLS.Utilities;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DLS.Core.UI
{
    [DisallowMultipleComponent]
    public class MenuController : MonoBehaviour
    {
        [SerializeField] private Page InitialPage;
        [SerializeField] private GameObject FirstFocusItem;
        [SerializeField] private Button continueButton, loadButton;
        [SerializeField] private TransitionParameters transitionParameters;

        private Canvas RootCanvas;

        private Stack<Page> PageStack = new ();

        private void Awake()
        {
            RootCanvas = GetComponent<Canvas>();
        }

        private void Start()
        {
            if (FirstFocusItem != null)
            {
                EventSystem.current.SetSelectedGameObject(FirstFocusItem);
            }

            if (InitialPage != null)
            {
                PushPage(InitialPage);
            }

            SetLoadContinueButtonInteractable();
        }

        public void SetLoadContinueButtonInteractable()
        {
            if (GameManager.Instance.IsCurrentState<PausedState>()) return;
            if(continueButton != null)
            {
                continueButton.interactable = DataPersistenceManager.instance.HasGameData();
            }

            if (loadButton != null)
            {
                loadButton.interactable = DataPersistenceManager.instance.HasGameData();
            }
        }

        private void OnCancel()
        {
            if (RootCanvas.enabled && RootCanvas.gameObject.activeInHierarchy)
            {
                if (PageStack.Count != 0)
                {
                    PopPage();
                }
            }
        }

        public bool IsPageInStack(Page Page)
        {
            return PageStack.Contains(Page);
        }

        public bool IsPageOnTopOfStack(Page Page)
        {
            return PageStack.Count > 0 && Page == PageStack.Peek();
        }

        public void PushPage(Page Page)
        {
            Page.Enter(true);

            if (PageStack.Count > 0)
            {
                Page currentPage = PageStack.Peek();

                if (currentPage.ExitOnNewPagePush)
                {
                    currentPage.Exit(false);
                }
            }

            PageStack.Push(Page);
        }

        public void PopPage()
        {
            if (PageStack.Count > 1)
            {
                Page page = PageStack.Pop();
                page.Exit(true);

                Page newCurrentPage = PageStack.Peek();
                if (newCurrentPage.ExitOnNewPagePush)
                {
                    newCurrentPage.Enter(false);
                }
            }
            else
            {
                Debug.LogWarning("Trying to pop a page but only 1 page remains in the stack!");
            }
        }

        public void PopAllPages()
        {
            for (int i = 1; i < PageStack.Count; i++)
            {
                PopPage();
            }
        }

        public void OpenURL(string url)
        {
            Application.OpenURL(url);
        }

        public void LoadScene(string sceneName)
        {
            SceneManager.LoadSceneAsync(sceneName);
        }

        public void ReloadScene()
        {
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        }

        public void Exit()
        {
            Application.Quit();
        }

        public void Unpause()
        {
            MessageSystem.MessageManager.SendImmediate(MessageChannels.GameFlow, new PauseMessage(false));
        }

        public void LoadMainMenu()
        {
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Level, new LevelMessage(LevelManager.Instance.CurrentLevel.ID, LevelManager.Instance.CurrentLevel.LevelName, LevelState.Unloading, Vector2.zero));
            MessageSystem.MessageManager.SendImmediate(MessageChannels.GameFlow, new GameStateMessage(GameManager.Instance.GetStateByType<MainMenuState>()));
        }

        public void ContinueGame(GameState state)
        {
            var player = PlayerManager.Instance.Player;
            var mostRecentPlayer = DataPersistenceManager.instance.dataHandler.GetMostRecentlyUpdatedPlayer();
            player.ID = mostRecentPlayer.playerID;
            player.ObjectName = mostRecentPlayer.playerName;
            player.transform.position = mostRecentPlayer.playerPosition;
            player.CurrentLevelID = mostRecentPlayer.levelID;
            player.CurrentLevelName = mostRecentPlayer.levelName;
            player.AchievementProgressList = mostRecentPlayer.playerAchievements;
            player.SavedGameObjects = mostRecentPlayer.savedGameObjects;
            
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Saves, new SaveLoadMessage(player.ID, player.ObjectName, SaveOperation.Load));
            MessageSystem.MessageManager.SendImmediate(MessageChannels.GameFlow, new GameStateMessage(state));
            MessageSystem.MessageManager.SendImmediate(MessageChannels.Level, new LevelMessage(player.CurrentLevelID, player.CurrentLevelName, LevelState.Loading, player.transform.position));
        }

        public void Transition()
        {
            // Create a new transition message
            var transitionMessage = new TransitionMessage(transitionParameters.transitionType, transitionParameters.slideDirection,
                transitionParameters.animationDurationInSeconds, transitionParameters.endEvent);

            // Send the message
            MessageSystem.MessageManager.SendImmediate(MessageChannels.UI, transitionMessage);
        }

        public void OpenAchievementsMenu()
        {
            MessageSystem.MessageManager.SendImmediate(MessageChannels.UI, new AchievementMenuMessage(true));
        }
    }
}
