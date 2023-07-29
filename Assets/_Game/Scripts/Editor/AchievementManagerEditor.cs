using DLS.Achievements;
using UnityEditor;
using UnityEngine;

namespace DLS.Game.Scripts.Editor
{
    [CustomEditor(typeof(AchievementManager))]
    public class AchievementManagerEditor : UnityEditor.Editor
    {
        private int selectedAchievementIndex = 0;

        public override void OnInspectorGUI()
        {
            AchievementManager manager = (AchievementManager)target;

            // Draw the default inspector
            DrawDefaultInspector();

            // Draw a button for each method
            if (GUILayout.Button("Load Achievements"))
            {
                manager.LoadAchievements();
            }
            
            // Draw a dropdown for resetting specific achievement
            if (manager.Manager.AchievementList != null && manager.Manager.AchievementList.Count > 0)
            {
                var achievementKeys = manager.Manager.AchievementList.ConvertAll(achievement => achievement.Key);
                selectedAchievementIndex = EditorGUILayout.Popup("Reset Achievement", selectedAchievementIndex, achievementKeys.ToArray());

                if (GUILayout.Button($"Reset {achievementKeys[selectedAchievementIndex]} Achievement"))
                {
                    manager.ResetAchievement(achievementKeys[selectedAchievementIndex]);
                }
            }

            if (GUILayout.Button("Reset All Achievements"))
            {
                manager.ResetAllAchievements();
            }
        }
    }
}