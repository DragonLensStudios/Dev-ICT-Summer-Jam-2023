using DLS.Game.Enums;
using UnityEngine.Events;

namespace DLS.Game.Messages
{
    public struct TransitionMessage
    {
        public TransitionType TransitionType { get;}
        public Direction SlideDirection { get; }
        public float AnimationDurationInSeconds { get; }
        public UnityEvent EndEvent { get; }

        public TransitionMessage(TransitionType transitionType, Direction slideDirection = Direction.None, float animationDurationInSeconds = 1, UnityEvent endEvent = null)
        {
            TransitionType = transitionType;
            SlideDirection = slideDirection;
            AnimationDurationInSeconds = animationDurationInSeconds;
            EndEvent = endEvent;
        }
    }
}