using System;
using System.Collections;
using System.Collections.Generic;
using DLS.Core;
using DLS.Game.Enums;
using DLS.Game.GameStates;
using DLS.Game.Managers;
using DLS.Game.Messages;
using DLS.Game.PLayers;
using DLS.Utilities;
using TMPro;
using UnityEngine;

public class BuilderTimerTextController : MonoBehaviour
{
    [field: SerializeField] public GameTime BuildTime;
    [field: SerializeField] public GameTime CountDownTimer;

    [field: SerializeField] public TMP_Text TimerText;
    [field: SerializeField] public TMP_Text ResourcesText;

    private PlayerController player;

    private void Awake()
    {
        player = PlayerManager.Instance.Player;
    }

    private void OnEnable()
    {
        CountDownTimer.SetTime((int)BuildTime.Year, (int)BuildTime.Month,(int) BuildTime.Week, (int)BuildTime.Day, (int)BuildTime.Hour, (int)BuildTime.Minute, (int)BuildTime.Second);
    }

    // Update is called once per frame
    void Update()
    {

        if (ResourcesText != null)
        {
            ResourcesText.text = $"Crystals: {player.Resources}";
        }
        
        CountDownTimer.ReverseTime(TimeType.Second);

        if (TimerText != null)
        {
            TimerText.text = $"Build Time Remaining: {(int)CountDownTimer.Minute}:{(int)CountDownTimer.Second}";
        }
        if (!CountDownTimer.CheckTime(_minute: 0, _second: 0)) return;
        MessageSystem.MessageManager.SendImmediate(MessageChannels.Builder, new BuilderEndMessage());
    }
}
