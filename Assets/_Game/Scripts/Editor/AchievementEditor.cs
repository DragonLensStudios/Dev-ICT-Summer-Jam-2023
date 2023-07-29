using DLS.Achievements;
using UnityEditor;
using UnityEngine;

namespace DLS.Game.Scripts.Editor
{
    [CustomEditor(typeof(Achievement))]
    public class AchievementEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            Achievement achievement = (Achievement)target;

            // Draw fields for editable properties
            achievement.Key = EditorGUILayout.TextField("Key", achievement.Key);
            string newDisplayName = EditorGUILayout.DelayedTextField("Display Name", achievement.DisplayName);
            achievement.Description = EditorGUILayout.TextField("Description", achievement.Description);
            achievement.UseIcon = EditorGUILayout.Toggle("Use Icon", achievement.UseIcon);
            achievement.LockedIcon = (Sprite)EditorGUILayout.ObjectField("Locked Icon", achievement.LockedIcon, typeof(Sprite), false);
            achievement.AchievedIcon = (Sprite)EditorGUILayout.ObjectField("Achieved Icon", achievement.AchievedIcon, typeof(Sprite), false);
            achievement.Spoiler = EditorGUILayout.Toggle("Spoiler", achievement.Spoiler);
            achievement.Progression = EditorGUILayout.Toggle("Progression", achievement.Progression);
            achievement.ProgressGoal = EditorGUILayout.FloatField("Progress Goal", achievement.ProgressGoal);
            achievement.NotificationFrequency = EditorGUILayout.FloatField("Notification Frequency", achievement.NotificationFrequency);
            achievement.ProgressSuffix = EditorGUILayout.TextField("Progress Suffix", achievement.ProgressSuffix);
            achievement.AchievedSound = EditorGUILayout.TextField("Achieved Sound", achievement.AchievedSound);
            achievement.ProgressMadeSound = EditorGUILayout.TextField("Progress Made Sound", achievement.ProgressMadeSound);

            // Check if display name has changed
            if (!newDisplayName.Equals(achievement.DisplayName))
            {
                // Rename asset file
                string path = AssetDatabase.GetAssetPath(achievement);
                AssetDatabase.RenameAsset(path, newDisplayName);
                achievement.DisplayName = newDisplayName;
            }

            // Apply changes
            if (GUI.changed)
            {
                EditorUtility.SetDirty(achievement);
                AssetDatabase.SaveAssets();
            }
        }
    }
}
