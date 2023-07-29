using System.Collections.Generic;
using DLS.Game.Enums;
using UnityEngine;

namespace DLS.Achievements
{
    [System.Serializable, CreateAssetMenu(fileName = "Achievement Manager Settings", menuName = "DLS/Game/Achievements/Achievement Manager Settings", order = 0)]
    public class AchievementManagerSettings : ScriptableObject
    {
        [field: SerializeField] public List<Achievement> AchievementList { get; set; }= new ();
    
        [Tooltip("The number of seconds an achievement will stay on the screen after being unlocked or progress is made.")]
        [field: SerializeField] public float DisplayTime { get; set; } = 3;
    
        [Tooltip("The total number of achievements which can be on the screen at any one time.")]
        [field: SerializeField] public int NumberOnScreen { get; set; } = 3;
    
        [Tooltip("If true, progress notifications will display their exact progress. If false it will show the closest bracket.")]
        [field: SerializeField] public bool ShowExactProgress { get; set; } = false;
    
        [Tooltip("If true, achievement unlocks/progress update notifications will be displayed on the player's screen.")]
        [field: SerializeField] public bool DisplayAchievements { get; set; }
    
        [Tooltip("The location on the screen where achievement notifications should be displayed.")]
        [field: SerializeField] public AchievementStackLocation StackLocation{ get; set; }

        [Tooltip("The message which will be displayed on the UI if an achievement is marked as a spoiler.")]
        [field: SerializeField] public string SpoilerAchievementMessage { get; set; } = "Hidden";
    
        [Tooltip("The sound which plays when an achievement is unlocked is displayed to a user. Sounds are only played when Display Achievements is true.")]
        [field: SerializeField] public string DefaultAchievedSound { get; set; }
    
        [Tooltip("The sound which plays when a progress update is displayed to a user. Sounds are only played when Display Achievements is true.")]
        [field: SerializeField] public string DefaultProgressMadeSound { get; set; }
    
        [Tooltip("If true, one achievement will be automatically unlocked once all others have been completed")]
        [field: SerializeField] public bool UseFinalAchievement { get; set; } = false;
    
        [Tooltip("The key of the final achievement")]
        [field: SerializeField] public string FinalAchievementKey { get; set; }
    
    }
}