using System;
using DLS.Game.Enums;
using DLS.Game.Messages;
using DLS.Utilities;
using UnityEngine;

namespace DLS.Game.GameStates
{
    
    [CreateAssetMenu(fileName = "GameOverState", menuName = "DLS/GameStates/GameOver", order = 2)]
    public class GameOverState : GameState
    {
        public GameObject gameOverCanvasPrefab;

        private GameObject gameoverCanvas;

        public override void Enter()
        {
            
            gameoverCanvas = null;
            if (gameoverCanvas == null)
            {
                gameoverCanvas = Instantiate(gameOverCanvasPrefab);
            }
            
            Debug.Log("Entered paused state");
        }

        public override void Exit()
        {
            if (gameoverCanvas != null)
            {
                Destroy(gameoverCanvas);
            }
            Debug.Log("Exited paused state");
        }

        public override void Update()
        {
            Debug.Log("Update paused state");
        }
    }
}