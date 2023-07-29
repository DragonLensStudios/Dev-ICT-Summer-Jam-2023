using UnityEngine;

namespace DLS.Game.GameStates
{
    [System.Serializable]
    public abstract class GameState : ScriptableObject
    {
        public abstract void Enter();
        public abstract void Exit();
        public abstract void Update();

        protected virtual void Setup()
        {
            // Perform state setup logic here
        }

        protected virtual void Teardown()
        {
            // Perform state teardown logic here
        }
    }
}