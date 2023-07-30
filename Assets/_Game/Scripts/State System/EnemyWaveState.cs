using DLS.Game.Enums;
using DLS.Utilities;
using UnityEngine;

namespace DLS.Game.GameStates
{
    [CreateAssetMenu(fileName = "EnemyWaveState", menuName = "DLS/GameStates/EnemyWaveState", order = 1)]
    public class EnemyWaveState : GameState
    {
        [SerializeField] private GameObject waveSpawnerPrefab;

        private GameObject waveSpawner;
        public override void Enter()
        {
            // MessageSystem.MessageManager.RegisterForChannel<WaveSpawningMessage>(MessageChannels.Spawning, WaveSpawningMessageHandler);
            waveSpawner = null;
            if (waveSpawner == null)
            {
                waveSpawner = Instantiate(waveSpawnerPrefab);
            }
            Debug.Log("Entered game playing state");
        }

        // private void WaveSpawningMessageHandler(MessageSystem.IMessageEnvelope message)
        // {
        //     throw new System.NotImplementedException();
        // }

        public override void Exit()
        {
            if (waveSpawner != null)
            {
                Destroy(waveSpawner);
            }
            Debug.Log("Exited game playing state");
        }

        public override void Update()
        {
            Debug.Log("Update game playing state");
        }
    }
}