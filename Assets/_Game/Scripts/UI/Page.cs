using DLS.Core.Audio;
using DLS.Game.Enums;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace DLS.Core.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    [DisallowMultipleComponent]
    public class Page : MonoBehaviour
    {
        public bool ExitOnNewPagePush = false;
        [SerializeField]
        private GameObject firstFocusItem;
        [SerializeField]
        private float AnimationSpeed = 1f;
        [SerializeField]
        private string EntrySfx, ExitSfx;
        [SerializeField]
        private EntryMode EntryMode = EntryMode.Slide;
        [SerializeField]
        private Direction EntryDirection = Direction.Left;
        [SerializeField]
        private EntryMode ExitMode = EntryMode.Slide;
        [SerializeField]
        private Direction ExitDirection = Direction.Left;
        [SerializeField]
        private UnityEvent PrePushAction;
        [SerializeField]
        private UnityEvent PostPushAction;
        [SerializeField]
        private UnityEvent PrePopAction;
        [SerializeField]
        private UnityEvent PostPopAction;

        private RectTransform RectTransform;
        private CanvasGroup CanvasGroup;
        private Coroutine AnimationCoroutine;
        private void Awake()
        {
            RectTransform = GetComponent<RectTransform>();
            CanvasGroup = GetComponent<CanvasGroup>();
            //Not sure if needed but it's not always working correctly to set focused item for event system.
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstFocusItem);
        }

        public void Enter(bool PlayAudio)
        {
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }
            if (firstFocusItem != null)
            {
                //Not sure if needed but it's not always working correctly to set focused item for event system.
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(firstFocusItem);
            }
            PrePushAction?.Invoke();

            switch(EntryMode)
            {
                case EntryMode.None:
                    PlayEntryClip(PlayAudio);
                    break;
                case EntryMode.Slide:
                    SlideIn(PlayAudio);
                    break;
                case EntryMode.Zoom:
                    ZoomIn(PlayAudio);
                    break;
                case EntryMode.Fade:
                    FadeIn(PlayAudio);
                    break;
            }
        }

        public void Exit(bool PlayAudio)
        {
            PrePopAction?.Invoke();

            switch (ExitMode)
            {
                case EntryMode.None:
                    PlayExitClip(PlayAudio);
                    break;
                case EntryMode.Slide:
                    SlideOut(PlayAudio);
                    break;
                case EntryMode.Zoom:
                    ZoomOut(PlayAudio);
                    break;
                case EntryMode.Fade:
                    FadeOut(PlayAudio);
                    break;
            }
            gameObject.SetActive(false);
        }

        private void SlideIn(bool PlayAudio)
        {
            if (AnimationCoroutine != null)
            {
                StopCoroutine(AnimationCoroutine);
            }
            AnimationCoroutine = StartCoroutine(AnimationHelper.SlideIn(RectTransform, EntryDirection, AnimationSpeed, PostPushAction));

            PlayEntryClip(PlayAudio);
        }

        private void SlideOut(bool PlayAudio)
        {
            if (AnimationCoroutine != null)
            {
                StopCoroutine(AnimationCoroutine);
            }
            AnimationCoroutine = StartCoroutine(AnimationHelper.SlideOut(RectTransform, ExitDirection, AnimationSpeed, PostPopAction));

            PlayExitClip(PlayAudio);
        }
    
        private void ZoomIn(bool PlayAudio)
        {
            if (AnimationCoroutine != null)
            {
                StopCoroutine(AnimationCoroutine);
            }
            AnimationCoroutine = StartCoroutine(AnimationHelper.ZoomIn(RectTransform, AnimationSpeed, PostPushAction));

            PlayEntryClip(PlayAudio);
        }

        private void ZoomOut(bool PlayAudio)
        {
            if (AnimationCoroutine != null)
            {
                StopCoroutine(AnimationCoroutine);
            }
            AnimationCoroutine = StartCoroutine(AnimationHelper.ZoomOut(RectTransform, AnimationSpeed, PostPopAction));

            PlayExitClip(PlayAudio);
        }

        private void FadeIn(bool PlayAudio)
        {
            if (AnimationCoroutine != null)
            {
                StopCoroutine(AnimationCoroutine);
            }
            AnimationCoroutine = StartCoroutine(AnimationHelper.FadeIn(CanvasGroup, AnimationSpeed, PostPushAction));

            PlayEntryClip(PlayAudio);
        }

        private void FadeOut(bool PlayAudio)
        {
            if (AnimationCoroutine != null)
            {
                StopCoroutine(AnimationCoroutine);
            }
            AnimationCoroutine = StartCoroutine(AnimationHelper.FadeOut(CanvasGroup, AnimationSpeed, PostPopAction));

            PlayExitClip(PlayAudio);
        }

        private void PlayEntryClip(bool PlayAudio)
        {
            if (PlayAudio && !string.IsNullOrWhiteSpace(EntrySfx))
            {
                AudioManager.instance.PlaySound(EntrySfx);
            }
        }
    
        private void PlayExitClip(bool PlayAudio)
        {
            if (PlayAudio && !string.IsNullOrWhiteSpace(ExitSfx))
            {
                AudioManager.instance.PlaySound(ExitSfx);
            }
        }
    }
}
