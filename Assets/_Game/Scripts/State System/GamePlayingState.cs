using UnityEngine;

namespace DLS.Game.GameStates
{
    [CreateAssetMenu(fileName = "GamePlayingState", menuName = "DLS/GameStates/GamePlayingState", order = 1)]
    public class GamePlayingState : GameState
    {
        [SerializeField] private GameObject inGameUICanvasPrefab;

        private GameObject inGameUICanvas;
        public override void Enter()
        {
            inGameUICanvas = null;
            if (inGameUICanvas == null)
            {
                inGameUICanvas = Instantiate(inGameUICanvasPrefab);
            }
            Debug.Log("Entered game playing state");
        }

        public override void Exit()
        {
            if (inGameUICanvas != null)
            {
                Destroy(inGameUICanvas);
            }
            Debug.Log("Exited game playing state");
        }

        public override void Update()
        {
            Debug.Log("Update game playing state");
        }
    }
}