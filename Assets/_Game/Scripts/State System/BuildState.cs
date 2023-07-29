using UnityEngine;

namespace DLS.Game.GameStates
{
    [CreateAssetMenu(fileName = "BuilderState", menuName = "DLS/GameStates/builderState", order = 1)]
    public class BuilderState : GameState
    {
        [SerializeField] private GameObject builderUICanvasPrefab;

        private GameObject builderUICanvas;
        public override void Enter()
        {
            builderUICanvas = null;
            if (builderUICanvas == null)
            {
                builderUICanvas = Instantiate(builderUICanvasPrefab);
            }
            Debug.Log("Entered building state");
        }

        public override void Exit()
        {
            if (builderUICanvas != null)
            {
                Destroy(builderUICanvas);
            }
            Debug.Log("Exited building state");
        }

        public override void Update()
        {
            Debug.Log("Update building state");
        }
    }
}