using DLS.Core;
using UnityEngine;

namespace DLS.Game.Managers
{
    public class TimeManager : MonoBehaviour
    {
        public static TimeManager Instance { get; private set; }
        
        [field: SerializeField] public GameTime CurrentTime { get; set; }
        [field: SerializeField] public GameTime BuildTimer { get; set; }

        /// <summary>
        /// Ensures that only one instance of TimeManager exists in the scene.
        /// </summary>
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        

    }
}