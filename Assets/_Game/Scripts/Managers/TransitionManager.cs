using System;
using DLS.Core.UI;
using DLS.Game.Enums;
using DLS.Game.Messages;
using DLS.Utilities;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace DLS.Game.Managers
{
    public class TransitionManager : MonoBehaviour
    {
        public static TransitionManager Instance { get; private set; }

        public TransitionType transitionType;
        public Direction slideDirection;
        public float animationDurationInSeconds = 1f;
        public UnityEvent onEnd;

        public RectTransform rectTransform;
        public CanvasGroup canvasGroup;
        public Image transitionImage;

        private void Awake()
        {
            canvasGroup = GetComponentInChildren<CanvasGroup>();
            transitionImage = GetComponentInChildren<Image>(true);
            rectTransform =transitionImage.GetComponent<RectTransform>();
            
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnEnable()
        {
            MessageSystem.MessageManager.RegisterForChannel<TransitionMessage>(MessageChannels.UI, HandleTransition);
        }
        
        private void OnDisable()
        {
            MessageSystem.MessageManager.UnregisterForChannel<TransitionMessage>(MessageChannels.UI, HandleTransition);
        }

        private void HandleTransition(MessageSystem.IMessageEnvelope message)
        {
            if (!message.Message<TransitionMessage>().HasValue) return;
            var data = message.Message<TransitionMessage>().Value;

            // Set the transition type, direction, and speed from the message
            transitionType = data.TransitionType;
            slideDirection = data.SlideDirection;
            animationDurationInSeconds = data.AnimationDurationInSeconds;
            
            onEnd = new UnityEvent();
            onEnd.AddListener(() => {
                data.EndEvent.Invoke();
                transitionImage.gameObject.SetActive(false);
            });
            
            // Start the transition
            StartTransition();
            
        }

        public void StartTransition()
        {
            // Enable the transition image
            transitionImage.gameObject.SetActive(true);

            switch (transitionType)
            {
                case TransitionType.None:
                    break;
                case TransitionType.ZoomIn:
                    StartCoroutine(AnimationHelper.ZoomIn(rectTransform, animationDurationInSeconds, onEnd));
                    break;
                case TransitionType.ZoomOut:
                    StartCoroutine(AnimationHelper.ZoomOut(rectTransform, animationDurationInSeconds, onEnd));
                    break;
                case TransitionType.FadeIn:
                    StartCoroutine(AnimationHelper.FadeOut(canvasGroup, animationDurationInSeconds, onEnd));
                    break;
                case TransitionType.FadeOut:
                    StartCoroutine(AnimationHelper.FadeIn(canvasGroup, animationDurationInSeconds, onEnd));
                    break;
                case TransitionType.SlideIn:
                    StartCoroutine(AnimationHelper.SlideIn(rectTransform, slideDirection, animationDurationInSeconds, onEnd));
                    break;
                case TransitionType.SlideOut:
                    StartCoroutine(AnimationHelper.SlideOut(rectTransform, slideDirection, animationDurationInSeconds, onEnd));
                    break;
            }
        }
    }
}
