using System.Collections;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class Fader : MonoBehaviour
    {
        private CanvasGroup canvasGroup;
        Coroutine currentRoutine;
        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }
        public Coroutine FadeOut(float time)
        {
            return Fade(1, time);
        }
        public Coroutine FadeIn(float time)
        {
            return Fade(0, time);
        }
        public Coroutine Fade(float target, float time)
        {
            if (currentRoutine != null)
            {
                StopCoroutine(currentRoutine);
            }
            currentRoutine = StartCoroutine(FadeRoutine(target, time));
            return currentRoutine;
        }
        public IEnumerator FadeRoutine(float target, float time)
        {
            while (!Mathf.Approximately(canvasGroup.alpha, target))
            {
                canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, target, Time.unscaledDeltaTime / time);
                yield return null;
            }
        }
        public void FadeOutImmediate()
        {
            canvasGroup.alpha = 1;
        }
    }
}