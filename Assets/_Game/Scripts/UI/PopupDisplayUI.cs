using System;
using System.Collections;
using DLS.Game.Enums;
using DLS.Game.Messages;
using DLS.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DLS.Core.UI
{
    public class PopupDisplayUI : MonoBehaviour
    {
        [SerializeField] private GameObject confirmDialog, textDialog, notificationPopup;
        [SerializeField] private TMP_Text confimPopupText, textPopupText, notificationPopupText;
        [SerializeField] private Button confirmButton, cancelButton, okButton;

        private EventSystem eventSystem;
        private Coroutine notificationCoroutine;
        

        private void OnEnable()
        {
            MessageSystem.MessageManager.RegisterForChannel<PopupMessage>(MessageChannels.UI, PopupMessageHandler);
            MessageSystem.MessageManager.RegisterForChannel<HidePopupMessage>(MessageChannels.UI, HidePopupMessageHandler);
        }



        private void OnDisable()
        {
            MessageSystem.MessageManager.UnregisterForChannel<PopupMessage>(MessageChannels.UI, PopupMessageHandler);
            MessageSystem.MessageManager.UnregisterForChannel<HidePopupMessage>(MessageChannels.UI, HidePopupMessageHandler);


        }

        private void Start()
        {
            eventSystem = FindFirstObjectByType<EventSystem>(FindObjectsInactive.Include);
        }

        public void ShowNotification(string text, float displayTime = 0f, PopupPosition position = PopupPosition.Bottom)
        {
            notificationPopup.SetActive(true);
            SetPopupPosition(notificationPopup, position);
            notificationPopupText.text = text;

            if (notificationCoroutine != null)
            {
                StopCoroutine(notificationCoroutine);
            }

            if (displayTime > 0f)
            {
                StartCoroutine(HideAfterDelay(notificationPopup, displayTime));
            }

        }

        public void ShowTextPopup(string text, UnityAction okAction = null, float displayTime = 0f, PopupPosition position = PopupPosition.Middle)
        {
            textDialog.gameObject.SetActive(true);
            SetPopupPosition(textDialog, position);
            MessageSystem.MessageManager.SendImmediate(MessageChannels.GameFlow, new PauseMessage(true));
            textPopupText.text = text;
            eventSystem.SetSelectedGameObject(okButton.gameObject);

            okButton.onClick.RemoveAllListeners();
            if (okAction != null)
            {
                okButton.onClick.AddListener(okAction);
                okButton.onClick.AddListener(HideTextDialog);
            }

            if (displayTime > 0f)
            {
                StartCoroutine(HideAfterDelay(textDialog, displayTime));
            }
        }

        public void ShowConfirmPopup(string text, UnityAction confirmAction = null, UnityAction cancelAction = null, float displayTime = 0f, PopupPosition position = PopupPosition.Middle)
        {
            confirmDialog.gameObject.SetActive(true);
            SetPopupPosition(confirmDialog, position);
            MessageSystem.MessageManager.SendImmediate(MessageChannels.GameFlow, new PauseMessage(true));
            confimPopupText.text = text;
            eventSystem.SetSelectedGameObject(confirmButton.gameObject);

            confirmButton.onClick.RemoveAllListeners();
            cancelButton.onClick.RemoveAllListeners();
            if (confirmAction != null)
            {
                confirmButton.onClick.AddListener(confirmAction);
                confirmButton.onClick.AddListener(HideConfrimDialog);
                confirmButton.gameObject.SetActive(true);
            }
            else
            {
                confirmButton.gameObject.SetActive(false);
            }

            if (cancelAction != null)
            {
                cancelButton.onClick.AddListener(cancelAction);
                cancelButton.onClick.AddListener(HideConfrimDialog);
                cancelButton.gameObject.SetActive(true);
            }
            else
            {
                cancelButton.gameObject.SetActive(false);
            }

            if (displayTime > 0f)
            {
                StartCoroutine(HideAfterDelay(confirmDialog, displayTime));
            }
        }

        private IEnumerator HideAfterDelay(GameObject dialog, float delay)
        {
            yield return new WaitForSeconds(delay);

            dialog.SetActive(false);
        }

        public void HideAllDialogs()
        {
            confirmDialog.SetActive(false);
            textDialog.SetActive(false);
            notificationPopup.SetActive(false);
            confirmButton.onClick.RemoveAllListeners();
            cancelButton.onClick.RemoveAllListeners();
            MessageSystem.MessageManager.SendImmediate(MessageChannels.GameFlow, new PauseMessage(false));
        }

        public void HideConfrimDialog()
        {
            confirmDialog.SetActive(false);
            confirmButton.onClick.RemoveAllListeners();
            cancelButton.onClick.RemoveAllListeners();
            MessageSystem.MessageManager.SendImmediate(MessageChannels.GameFlow, new PauseMessage(false));
        }

        public void HideTextDialog()
        {
            textDialog.SetActive(false);
            okButton.onClick.RemoveAllListeners();
            MessageSystem.MessageManager.SendImmediate(MessageChannels.GameFlow, new PauseMessage(false));
        }

        public void HideNotificationPopup()
        {
            notificationPopup.SetActive(false);
        }
        
        private void PopupMessageHandler(MessageSystem.IMessageEnvelope message)
        {
            if(!message.Message<PopupMessage>().HasValue) return;
            var data = message.Message<PopupMessage>().Value;
            switch (data.PopupType)
            {
                case PopupType.Confirm:
                    ShowConfirmPopup(data.Message,data.ConfirmAction, data.CancelAction, data.DisplayTime, data.PopupPosition);
                    break;
                case PopupType.Message:
                    ShowTextPopup(data.Message, data.OkAction, data.DisplayTime,data.PopupPosition);
                    break;
                case PopupType.Notification:
                    ShowNotification(data.Message, data.DisplayTime, data.PopupPosition);
                    break;
            }
        }
        
        private void HidePopupMessageHandler(MessageSystem.IMessageEnvelope message)
        {
            if(!message.Message<HidePopupMessage>().HasValue) return;
            var data = message.Message<HidePopupMessage>().Value;

            switch (data.PopupType)
            {
                case PopupType.Confirm:
                    HideConfrimDialog();
                    break;
                case PopupType.Message:
                    HideTextDialog();
                    break;
                case PopupType.Notification:
                    HideNotificationPopup();
                    break;
            }

        }
        
        private void SetPopupPosition(GameObject popup, PopupPosition position)
        {
            RectTransform rectTransform = popup.GetComponent<RectTransform>();
            RectTransform parentRectTransform = rectTransform.parent.GetComponent<RectTransform>();
    
            switch (position)
            {
                case PopupPosition.Top:
                    rectTransform.anchoredPosition = new Vector2(0, parentRectTransform.rect.height / 2 - rectTransform.rect.height / 2);
                    break;
                case PopupPosition.Middle:
                    rectTransform.anchoredPosition = Vector2.zero;
                    break;
                case PopupPosition.Bottom:
                    rectTransform.anchoredPosition = new Vector2(0, -parentRectTransform.rect.height / 2 + rectTransform.rect.height / 2);
                    break;
            }
        }
    }
}
