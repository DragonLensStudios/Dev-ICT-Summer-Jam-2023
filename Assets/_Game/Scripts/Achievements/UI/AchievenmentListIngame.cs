using System.Linq;
using DLS.Core.Input;
using DLS.Game.Enums;
using DLS.Game.GameStates;
using DLS.Game.Managers;
using DLS.Game.Messages;
using DLS.Utilities;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

namespace DLS.Achievements
{
    /// <summary>
    /// Add list of achievements to screen
    /// </summary>
    public class AchievenmentListIngame : MonoBehaviour
    {
        [field: SerializeField] public AchievementManagerSettings Manager { get; set; }
        [field: SerializeField] public GameObject ScrollContent { get; set; }
        [field: SerializeField] public GameObject Prefab { get; set; }
        [field: SerializeField] public GameObject Menu { get; set; }
        [field: SerializeField] public TMP_Dropdown Filter { get; set; }
        [field: SerializeField] public TMP_Text CountText { get; set; }
        [field: SerializeField] public TMP_Text CompleteText { get; set; }
        [field: SerializeField] public Scrollbar Scrollbar { get; set; }
        [field: SerializeField] public bool UseInputToOpenAchievementsList { get; set; } = true;
        
        private PlayerInputActions playerInput;

        private void Awake()
        {
            playerInput = new PlayerInputActions();
        }

        private void OnEnable()
        {
            playerInput.Enable();
            playerInput.Player.OpenAchievementMenu.performed += OpenAchievementMenuOnperformed;
            MessageSystem.MessageManager.RegisterForChannel<AchievementMenuMessage>(MessageChannels.UI, MenuMessageHandler);
        }



        private void OnDisable()
        {
            playerInput.Disable();
            playerInput.Player.OpenAchievementMenu.performed -= OpenAchievementMenuOnperformed;
            MessageSystem.MessageManager.UnregisterForChannel<AchievementMenuMessage>(MessageChannels.UI, MenuMessageHandler);

        }

        private bool MenuOpen = false;
        
        /// <summary>
        /// Adds all achievements to the UI based on a filter
        /// </summary>
        /// <param name="Filter">Filter to use (All, Achieved or Unachieved)</param>
        public void AddAchievements(string Filter)
        {  
            foreach (Transform child in ScrollContent.transform)
            {
                Destroy(child.gameObject);
            }
            int AchievedCount = AchievementManager.Instance.GetAchievedCount();

            CountText.text = "" + AchievedCount + " / " + Manager.AchievementList.Count(x => !x.Key.Equals(Manager.FinalAchievementKey));
            CompleteText.text = "Complete (" + AchievementManager.Instance.GetAchievedPercentage() + "%)";

            for (int i = 0; i < Manager.AchievementList.Count; i ++)
            {
                var achievement = Manager.AchievementList[i];
                var achievementProgress = AchievementManager.Instance.PlayerAchievementProgress.FirstOrDefault(x => x.AchievementKey.Equals(achievement.Key));
                if((Filter.Equals("All")) || (Filter.Equals("Achieved") && achievementProgress.Achieved) || (Filter.Equals("Unachieved") && !achievementProgress.Achieved))
                {
                    AddAchievementToUI(Manager.AchievementList[i]);
                }
            }
            Scrollbar.value = 1;
        }

        public void AddAchievementToUI(Achievement Achievement)
        {
            UIAchievement UIAchievement = Instantiate(Prefab, new Vector3(0f, 0f, 0f), Quaternion.identity).GetComponent<UIAchievement>();
            UIAchievement.Set(Achievement);
            UIAchievement.transform.SetParent(ScrollContent.transform);
        }
        /// <summary>
        /// Filter out a set of locked or unlocked achievements
        /// </summary>
        public void ChangeFilter ()
        {
            AddAchievements(Filter.options[Filter.value].text);
        }

        /// <summary>
        /// Closes the UI window.
        /// </summary>
        public void CloseWindow()
        {
            MenuOpen = false;
            Menu.SetActive(MenuOpen);
        }
        /// <summary>
        /// Opens the UI window.
        /// </summary>
        public void OpenWindow()
        {
            MenuOpen = true;
            Menu.SetActive(MenuOpen);
            AddAchievements("All");
        }
        /// <summary>
        /// Toggles the state of the UI window open or closed
        /// </summary>
        public void ToggleWindow()
        {
            if (MenuOpen){
                CloseWindow();
            }
            else{
                OpenWindow();
            }
        }
     
        private void OpenAchievementMenuOnperformed(InputAction.CallbackContext input)
        {
            if (!UseInputToOpenAchievementsList || GameManager.Instance.CurrentState is MainMenuState) return;
            ToggleWindow();
            MessageSystem.MessageManager.SendImmediate(MessageChannels.GameFlow, new PauseMessage(MenuOpen));
        }
        
        private void MenuMessageHandler(MessageSystem.IMessageEnvelope message)
        {
            if(!message.Message<AchievementMenuMessage>().HasValue) return;
            var data = message.Message<AchievementMenuMessage>().Value;
            if(data.ShowMenu) OpenWindow();
            else CloseWindow();
        }
    }
}
