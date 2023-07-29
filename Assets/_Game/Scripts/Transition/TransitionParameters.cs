using System;
using DLS.Game.Enums;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace DLS.Game.Transition
{
    [Serializable]
    public class TransitionParameters
    {
        public TransitionType transitionType;
        public Direction slideDirection = Direction.None;
        public float animationDurationInSeconds = 1;
        public UnityEvent endEvent;
    }
}