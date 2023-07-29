using DLS.Core;
using DLS.Core.Input;
using DLS.Game.Enums;
using DLS.Game.Messages;
using DLS.Game.PLayers;
using DLS.Game.Utilities;
using DLS.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DLS.Game.Saves
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class SavePoint : MonoBehaviour
    {
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
        }

        private void OnDisable()
        {
            playerInput.Disable();
            playerInput.Player.Interact.performed -= InteractOnperformed;
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (!col.CompareTag("Player")) return;
            player = col.GetComponent<PlayerController>();
            
            if (actionReference == null)
            {
                Debug.LogError("Action reference or button text is not assigned.");
                return;
            }
            string buttonName = InputHelper.GetButtonNameForAction(actionReference);
            MessageSystem.MessageManager.SendImmediate(MessageChannels.UI, new PopupMessage($"Press {buttonName} To Save", PopupType.Notification));
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
                MessageSystem.MessageManager.SendImmediate(MessageChannels.UI, new PopupMessage("Would you like to save?", PopupType.Confirm, PopupPosition.Middle, 0, null, () => {},
                    () =>
                    {
                        MessageSystem.MessageManager.SendImmediate(MessageChannels.Saves,  new SaveLoadMessage(player.ID, ((ActorController)player).ObjectName, SaveOperation.Save));
                    }));
            }
        }
    }
}
