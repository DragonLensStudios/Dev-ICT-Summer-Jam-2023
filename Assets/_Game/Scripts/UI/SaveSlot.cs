using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DLS.Core.UI;
using DLS.Game.Enums;
using DLS.Game.GameStates;
using DLS.Game.Managers;
using DLS.Game.Messages;
using DLS.Game.PLayers;
using DLS.Game.Transition;
using DLS.Game.Utilities;
using DLS.Utilities;
using TMPro;
using UnityEngine;

public class SaveSlot : MonoBehaviour
{
    [SerializeField] private SerializableGuid playerID;
    [SerializeField] private string playerName;
    [SerializeField] private TMP_Text playerNameText, timestampText;
    [SerializeField] private GameState state;
    [SerializeField] private TransitionParameters transitionParameters;

    private PlayerController player;
    
    public SerializableGuid PlayerID { get => playerID; set => playerID = value; }
    public string PlayerName { get => playerName; set => playerName = value; }
    public TMP_Text PlayerNameText { get => playerNameText; set => playerNameText = value; }
    public TMP_Text TimestampText { get => timestampText; set => timestampText = value; }

    private void Start()
    {
        player = PlayerManager.Instance.Player;
    }

    public void LoadSave()
    {
        MessageSystem.MessageManager.SendImmediate(MessageChannels.Saves, new SaveLoadMessage(PlayerID, playerName, SaveOperation.Load));
        MessageSystem.MessageManager.SendImmediate(MessageChannels.GameFlow, new GameStateMessage(state));
        MessageSystem.MessageManager.SendImmediate(MessageChannels.Level, new LevelMessage(player.CurrentLevelID, player.CurrentLevelName, LevelState.Loading, player.transform.position));
    }

    public void DeleteSave()
    {
        MessageSystem.MessageManager.SendImmediate(MessageChannels.UI, 
            new PopupMessage("Are you sure you want to delete this save?", PopupType.Confirm, PopupPosition.Middle, 0f, null, () => { }, () =>
            {
                MessageSystem.MessageManager.SendImmediate(MessageChannels.Saves, new SaveLoadMessage(PlayerID, playerName, SaveOperation.Delete));
            }));
    }

    public void Transition()
    {
        // Create a new transition message
        var transitionMessage = new TransitionMessage(transitionParameters.transitionType, transitionParameters.slideDirection,
            transitionParameters.animationDurationInSeconds, transitionParameters.endEvent);

        // Send the message
        MessageSystem.MessageManager.SendImmediate(MessageChannels.UI, transitionMessage);
    }
}
