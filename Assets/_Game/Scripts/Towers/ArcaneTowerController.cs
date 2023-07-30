using DLS.Utilities;
using System.Collections.Generic;
using UnityEngine;
using DLS.Game.Enums;
using DLS.Game.Messages;
using DLS.Core.Data_Persistence.Extensions;
using DLS.Core.Input;
using UnityEngine.InputSystem;
using DLS.Game.PLayers;
using DLS.Core;
using DLS.Game.Utilities;

namespace DLS.Game.Towers
{
    public class ArcaneTowerController : MonoBehaviour
    {

        [field: SerializeField] public int CurrentStack = 1;
        [field: SerializeField] public int MaxStack = 3;
        [field: SerializeField] public List<GameObject> Crystals = new List<GameObject>();


        [SerializeField] private InputActionReference actionReference;
        private PlayerInputActions playerInput;
        private PlayerController player;

        private void Awake()
        {
            playerInput = new PlayerInputActions();
        }

        private void OnEnable()
        {
            playerInput.Enable();
            playerInput.Player.Interact.performed += InteractOnperformed;
            MessageSystem.MessageManager.RegisterForChannel<CrystalSpawnerMessage>(MessageChannels.Spawning, CrystalSpawnerMessageHandler);

        }

        private void OnDisable()
        {
            playerInput.Disable();
            playerInput.Player.Interact.performed -= InteractOnperformed;
            MessageSystem.MessageManager.UnregisterForChannel<CrystalSpawnerMessage>(MessageChannels.Spawning, CrystalSpawnerMessageHandler);

        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (!col.CompareTag("Player")) return;
            player = col.GetComponent<PlayerController>();
            if(player == null) return;

            if (actionReference == null)
            {
                Debug.LogError("Action reference or button text is not assigned.");
                return;
            }
            if(player.Resources < CurrentStack + 1 && CurrentStack != MaxStack)
            {
                MessageSystem.MessageManager.SendImmediate(MessageChannels.UI, new PopupMessage($"Insufficent Resources To Stack", PopupType.Notification, PopupPosition.Top));
               
            }
            else if(CurrentStack == MaxStack)
            {
                MessageSystem.MessageManager.SendImmediate(MessageChannels.UI, new PopupMessage($"Tower Complete", PopupType.Notification, PopupPosition.Top));
            }
            else if(player.Resources >= CurrentStack + 1)
            {
                string buttonName = InputHelper.GetButtonNameForAction(actionReference);
                MessageSystem.MessageManager.SendImmediate(MessageChannels.UI, new PopupMessage($"Press {buttonName} To Stack Tower", PopupType.Notification, PopupPosition.Top));
            }
        }

        private void OnTriggerExit2D(Collider2D col)
        {
            if (!col.CompareTag("Player")) return;
            player = null;
            MessageSystem.MessageManager.SendImmediate(MessageChannels.UI, new HidePopupMessage(PopupType.Notification));
        }

        private void InteractOnperformed(InputAction.CallbackContext input)
        {
            if (player != null)
            {
                MessageSystem.MessageManager.SendImmediate(MessageChannels.UI, new HidePopupMessage(PopupType.Notification));
                if(player.Resources >= CurrentStack + 1 && CurrentStack < MaxStack)
                {
                    CurrentStack++;
                    player.Resources -= CurrentStack;
                    MessageSystem.MessageManager.SendImmediate(MessageChannels.Spawning, new CrystalSpawnerMessage(gameObject, CurrentStack, MaxStack, null));
                    MessageSystem.MessageManager.SendImmediate(MessageChannels.UI, new PopupMessage($"Stacked Tower", PopupType.Notification));
                }
            }
        }

        private void CrystalSpawnerMessageHandler(MessageSystem.IMessageEnvelope message)
        {
            if (!message.Message<CrystalSpawnerMessage>().HasValue) return;
            var data = message.Message<CrystalSpawnerMessage>().Value;
            var currentTowerID = gameObject.GetObjectInstanceID();
            if (data.TowerPrefab.GetObjectInstanceID() != currentTowerID) return;
            CurrentStack = data.CurrentStack;
            MaxStack = data.MaxStack;
            for (int i = 0; i < data.CurrentStack; i++)
            {
                if (Crystals.Count < i + 1) continue;
                if (Crystals[i].activeSelf) continue;
                Crystals[i].SetActive(true);
            }
        }

    }
}