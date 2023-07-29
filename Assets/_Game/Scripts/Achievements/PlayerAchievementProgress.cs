using System.Linq;
using UnityEngine;

namespace DLS.Achievements
{
    [System.Serializable]
    public class PlayerAchievementProgress
    {
        [Tooltip("The Achievement key to match")]
        [field: SerializeField] public string AchievementKey { get; set; }
        
        [Tooltip("The progress for the achievement")]
        [field: SerializeField] public float Progress { get; set; } = 0f;   
        
        [Tooltip("The last progress for the achievement")]
        [field: SerializeField] public float LastProgressUpdate { get; set; } = 0f;
        
        [Tooltip("When the achievement is achieved this is true.")]
        [field: SerializeField] public bool Achieved { get; set; } = false;
        
        public PlayerAchievementProgress(string key)
        {
            AchievementKey = key;
        }

        public void AddProgress(float value)
        {
            var achievement = AchievementManager.Instance.Manager.AchievementList.FirstOrDefault(x => x.Key.Equals(AchievementKey));
            if(achievement == null) return;
            Progress += value;
            if(Progress >= achievement.ProgressGoal)
            {
                Unlock();
            }
        }

        public void SubtractProgress(float value)
        {
            Progress -= value;
            if (Progress < 0)
            {
                Progress = 0;
            }
        }

        public void SetProgress(float value)
        {
            var achievement = AchievementManager.Instance.Manager.AchievementList.FirstOrDefault(x => x.Key.Equals(AchievementKey));
            if(achievement == null) return;
            Progress = value;
            if (Progress >= achievement.ProgressGoal)
            {
                Unlock();
            }
        }

        public void Unlock()
        {
            Achieved = true;
        }

        public void Lock()
        {
            Achieved = false;
            Progress = 0;
        }
    }
}