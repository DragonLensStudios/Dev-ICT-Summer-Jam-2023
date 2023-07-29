using System;
using UnityEngine;

namespace DLS.Achievements
{
    /// <summary>
    /// The Achievement data and information
    /// </summary>
    [System.Serializable, CreateAssetMenu(fileName = "Achievement", menuName = "DLS/Game/Achievements/Achievement", order = 1)]
    public class Achievement : ScriptableObject
    {
        [Tooltip("Name used to unlock/set achievement progress")]
        [field: SerializeField] public string Key { get; set; }
    
        [Tooltip("The display name for an achievement. Shown to the user on the UI.")]
        [field: SerializeField] public string DisplayName { get; set; }
    
        [Tooltip("Description for an achievement. Shown to the user on the UI.")]
        [field: SerializeField] public string Description { get; set; }
        
        [Tooltip("If true, the lock/achieved icon will be displayed")]
        [field: SerializeField] public bool UseIcon { get; set; }
    
        [Tooltip("The icon which will be displayed when the achievement is locked")]
        [field: SerializeField] public Sprite LockedIcon { get; set; }
    
        [Tooltip("The icon which will be displayed when the achievement is  Achieved")]
        [field: SerializeField] public Sprite AchievedIcon { get; set; }
    
        [Tooltip("Treat the achievement as a spoiler for the game. Hidden from player until unlocked.")]
        [field: SerializeField] public bool Spoiler { get; set; }
    
        [Tooltip("If true, this achievement will count to a certain amount before unlocking. E.g. race a total of 500 km, collect 10 coins or reach a high score of 25.")]
        [field: SerializeField] public bool Progression { get; set; }
    
        [Tooltip("The goal which must be reached for the achievement to unlock.")]
        [field: SerializeField] public float ProgressGoal { get; set; }
        
        [Tooltip("The rate that progress updates will be displayed on the screen e.g. Progress goal = 100 and Notification Frequency = 25. In this example, the progress will be displayed at 25,50,75 and 100.")]
        [field: SerializeField] public float NotificationFrequency { get; set; }
    
        [Tooltip("A string which will be displayed with a progress achievement e.g. $, KM, Miles etc")]
        [field: SerializeField] public string ProgressSuffix { get; set; }
    
        [Tooltip("The sound which plays when an achievement is unlocked is displayed to a user. Sounds are only played when Display Achievements is true.")]
        [field: SerializeField] public string AchievedSound { get; set; }
    
        [Tooltip("The sound which plays when a progress update is displayed to a user. Sounds are only played when Display Achievements is true.")]
        [field: SerializeField] public string ProgressMadeSound { get; set; }
    }
}