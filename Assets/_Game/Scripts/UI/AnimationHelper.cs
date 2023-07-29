using System.Collections;
using DLS.Game.Enums;
using UnityEngine;
using UnityEngine.Events;

namespace DLS.Core.UI
{
    public class AnimationHelper
    {
        public static IEnumerator ZoomIn(RectTransform Transform, float duration, UnityEvent OnEnd)
        {
            float time = 0;
            while (time < 1)
            {
                Transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, time);
                yield return null;
                time += Time.deltaTime / duration;
            }

            Transform.localScale = Vector3.one;

            OnEnd?.Invoke();
        }

        public static IEnumerator ZoomOut(RectTransform Transform, float duration, UnityEvent OnEnd)
        {
            float time = 0;
            while (time < 1)
            {
                Transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, time);
                yield return null;
                time += Time.deltaTime / duration;
            }

            Transform.localScale = Vector3.zero;
            OnEnd?.Invoke();
        }

        public static IEnumerator FadeIn(CanvasGroup CanvasGroup, float duration, UnityEvent OnEnd)
        {
            CanvasGroup.blocksRaycasts = true;
            CanvasGroup.interactable = true;

            float time = 0;
            while (time < 1)
            {
                CanvasGroup.alpha = Mathf.Lerp(0, 1, time);
                yield return null;
                time += Time.deltaTime / duration;
            }

            CanvasGroup.alpha = 1;
            OnEnd?.Invoke();
        }

        public static IEnumerator FadeOut(CanvasGroup CanvasGroup, float duration, UnityEvent OnEnd)
        {
            CanvasGroup.blocksRaycasts = false;
            CanvasGroup.interactable = false;

            float time = 0;
            while (time < 1)
            {
                CanvasGroup.alpha = Mathf.Lerp(1, 0, time);
                yield return null;
                time += Time.deltaTime / duration;
            }

            CanvasGroup.alpha = 0;
            OnEnd?.Invoke();
        }

        public static IEnumerator SlideIn(RectTransform Transform, Direction Direction, float duration, UnityEvent OnEnd)
        {
            Vector2 startPosition;
            switch (Direction)
            {
                case Direction.Up:
                    startPosition = new Vector2(0, -Screen.height);
                    break;
                case Direction.Right:
                    startPosition = new Vector2(-Screen.width, 0);
                    break;
                case Direction.Down:
                    startPosition = new Vector2(0, Screen.height);
                    break;
                case Direction.Left:
                    startPosition = new Vector2(Screen.width, 0);
                    break;
                default:
                    startPosition = new Vector2(0, -Screen.height);
                    break;
            }

            float time = 0;
            while (time < 1)
            {
                Transform.anchoredPosition = Vector2.Lerp(startPosition, Vector2.zero, time);
                yield return null;
                time += Time.deltaTime / duration;
            }

            Transform.anchoredPosition = Vector2.zero;
            OnEnd?.Invoke();
        }

        public static IEnumerator SlideOut(RectTransform Transform, Direction Direction, float duration, UnityEvent OnEnd)
        {
            Vector2 endPosition;
            switch (Direction)
            {
                case Direction.Up:
                    endPosition = new Vector2(0, Screen.height);
                    break;
                case Direction.Right:
                    endPosition = new Vector2(Screen.width, 0);
                    break;
                case Direction.Down:
                    endPosition = new Vector2(0, -Screen.height);
                    break;
                case Direction.Left:
                    endPosition = new Vector2(-Screen.width, 0);
                    break;
                default:
                    endPosition = new Vector2(0, Screen.height);
                    break;
            }

            float time = 0;
            while (time < 1)
            {
                Transform.anchoredPosition = Vector2.Lerp(Vector2.zero, endPosition, time);
                yield return null;
                time += Time.deltaTime / duration;
            }

            Transform.anchoredPosition = endPosition;
            OnEnd?.Invoke();
        }
    }
}
