using DLS.Game.Enums;
using DLS.Game.Messages;
using DLS.Utilities;
using UnityEngine;

namespace DLS.Game.GameStates
{
    [CreateAssetMenu(fileName = "MainMenuState", menuName = "DLS/GameStates/MainMenuState", order = 0)]
    public class MainMenuState : GameState
    {
        public GameObject mainMenuCanvasPrefab;

        private GameObject mainMenu;
        
        public override void Enter()
        {
            mainMenu = null;
            if (mainMenu == null)
            {
                mainMenu = Instantiate(mainMenuCanvasPrefab);
            }
            
            MessageSystem.MessageManager.SendImmediate(MessageChannels.GameFlow, new PauseMessage(true));

            Debug.Log("Entered main menu state");
        }

        public override void Exit()
        {
            if (mainMenu != null)
            {
                Destroy(mainMenu);
            }
            
            MessageSystem.MessageManager.SendImmediate(MessageChannels.GameFlow, new PauseMessage(false));
            
            Debug.Log("Exited main menu state");

        }

        public override void Update()
        {
            Debug.Log("Update main menu state");
        }
    }
}