using System;
using System.Collections;
using System.Collections.Generic;
using DLS.Core;
using DLS.Core.Input;
using DLS.Game.Enums;
using DLS.Game.GameStates;
using DLS.Game.Managers;
using DLS.Game.Messages;
using DLS.Game.PLayers;
using DLS.Game.Utilities;
using DLS.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

public class SkullController : MonoBehaviour
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
        MessageSystem.MessageManager.SendImmediate(MessageChannels.UI, new PopupMessage($"Press {buttonName} To Start Building", PopupType.Notification, PopupPosition.Top));
    }
    
    private void OnTriggerExit2D(Collider2D col)
    {
        if (!col.CompareTag("Player")) return;
        player = null;
        MessageSystem.MessageManager.SendImmediate(MessageChannels.UI, new HidePopupMessage(PopupType.Notification));
    }
    
    private void InteractOnperformed(InputAction.CallbackContext input)
    {
        if (player == null) return;
        if(GameManager.Instance.IsCurrentState<BuilderState>()) return;
        var builderState = GameManager.Instance.GetStateByType<BuilderState>();
        MessageSystem.MessageManager.SendImmediate(MessageChannels.GameFlow, new GameStateMessage(builderState));
    }
}
