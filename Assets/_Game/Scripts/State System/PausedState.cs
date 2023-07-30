using UnityEngine;

namespace DLS.Game.GameStates
{
    [CreateAssetMenu(fileName = "PausedState", menuName = "DLS/GameStates/Paused", order = 2)]
    public class PausedState : GameState
    {
        public GameObject pauseMenuCanvasPrefab;

        private GameObject pauseMenu;

        public override void Enter()
        {
            
            pauseMenu = null;
            if (pauseMenu == null)
            {
                pauseMenu = Instantiate(pauseMenuCanvasPrefab);
            }
            
            Debug.Log("Entered paused state");
        }

        public override void Exit()
        {
            if (pauseMenu != null)
            {
                Destroy(pauseMenu);
            }
            Debug.Log("Exited paused state");
        }

        public override void Update()
        {
            Debug.Log("Update paused state");
        }
    }
}