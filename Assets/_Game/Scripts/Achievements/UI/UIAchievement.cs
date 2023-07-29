using System.Collections;
using System.Linq;
using DLS.Game.Managers;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DLS.Achievements
{
    /// <summary>
    /// Defines the logic behind a single achievement on the UI
    /// </summary>
    public class UIAchievement : MonoBehaviour
    {
        [field: SerializeField] public AchievementManagerSettings Manager { get; set; }
        [field: SerializeField] private TMP_Text Title { get; set; }
        [field: SerializeField] private TMP_Text Description { get; set; }
        [field: SerializeField] private TMP_Text Percent { get; set; }
        [field: SerializeField] private Image OverlayIcon { get; set; }
        [field: SerializeField] private Image ProgressBar { get; set; }
        [field: SerializeField] private GameObject SpoilerOverlay { get; set; }
        [field: SerializeField] private TMP_Text SpoilerText { get; set; }
        
        [HideInInspector] public AchievementStack AS;

        /// <summary>
        /// Destroy object after a certain amount of time
        /// </summary>
        public void StartDeathTimer ()
        {
            StartCoroutine(Wait());
        }

        /// <summary>
        /// Add information  about an Achievement to the UI elements
        /// </summary>
        public void Set (Achievement achievement)
        {
            var achievementProgress  = PlayerManager.Instance.Player.AchievementProgressList.FirstOrDefault(x => x.AchievementKey.Equals(achievement.Key));
            if(achievement.Spoiler && !achievementProgress.Achieved)
            {
                SpoilerOverlay.SetActive(true);
                SpoilerText.text = Manager.SpoilerAchievementMessage;
            }
            else
            {
                Title.text = achievement.DisplayName;
                Description.text = achievement.Description;

                if (achievement.UseIcon && !achievementProgress.Achieved)
                {
                    if (achievement.LockedIcon == null) return;
                    OverlayIcon.gameObject.SetActive(true);
                    OverlayIcon.sprite = achievement.LockedIcon;
                }
                else if (achievement.UseIcon && achievementProgress.Achieved)
                {
                    if (achievement.AchievedIcon == null) return;
                    OverlayIcon.gameObject.SetActive(true);
                    OverlayIcon.sprite = achievement.AchievedIcon;
                }

                if (achievement.Progression)
                {
                    float CurrentProgress = Manager.ShowExactProgress ? achievementProgress.Progress : (achievementProgress.LastProgressUpdate * achievement.NotificationFrequency);
                    float DisplayProgress = achievementProgress.Achieved ? achievement.ProgressGoal : CurrentProgress;

                    if (achievementProgress.Achieved)
                    {
                        Percent.text = achievement.ProgressGoal + achievement.ProgressSuffix + " / " + achievement.ProgressGoal + achievement.ProgressSuffix + " (Achieved)";
                    }
                    else
                    {
                        Percent.text = DisplayProgress + achievement.ProgressSuffix +  " / " + achievement.ProgressGoal + achievement.ProgressSuffix;
                    }

                    ProgressBar.fillAmount = DisplayProgress / achievement.ProgressGoal;
                }
                else //Single Time
                {
                    ProgressBar.fillAmount = achievementProgress.Achieved ? 1 : 0;
                    Percent.text = achievementProgress.Achieved ? "(Achieved)" : "(Locked)";
                }
            }
        }

        private IEnumerator Wait ()
        {
            yield return new WaitForSeconds(Manager.DisplayTime);
            GetComponent<Animator>().SetTrigger("ScaleDown");
            yield return new WaitForSeconds(0.1f);
            AS.CheckBackLog();
            Destroy(gameObject);
        }
    }

}
