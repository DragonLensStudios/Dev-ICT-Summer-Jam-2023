using System;
using DLS.Achievements;
using DLS.Core;
using DLS.Game.Enums;
using DLS.Game.Messages;
using DLS.Utilities;
using UnityEngine;

namespace DLS.Game.Achievements
{
    [RequireComponent(typeof(Collider2D))]
    public class AchievementTrigger : MonoBehaviour
    {
        [field: SerializeField] public Achievement Achievement { get; set; }
        [field: SerializeField] public float Progress { get; set; }
        
        [field: SerializeField] public bool Unlock { get; set; }

        private Collider2D achievementCollider;

        private void Awake()
        {
            achievementCollider = GetComponent<Collider2D>();
            achievementCollider.isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.gameObject.CompareTag("Player"))
            {
                if (Achievement.Progression)
                {
                    MessageSystem.MessageManager.SendImmediate(MessageChannels.Achievement, new AchievementMessage(Achievement.Key, AchievementOperator.Add, Progress));
                }
                else
                {
                    MessageSystem.MessageManager.SendImmediate(MessageChannels.Achievement, new AchievementMessage(Achievement.Key, AchievementOperator.Unlock));
                }
            }
        }
    }
}