using System;
using UnityEngine;

using Cysharp.Threading.Tasks;

using RottenNoble.Core;
using RottenNoble.Core.UI;

namespace RottenNoble.Splash.UI
{
    /// <summary>
    /// Splash 씬 View — 로고 페이드인 / 페이드아웃 담당
    /// </summary>
    public class SplashView : ViewBase
    {
        [Header("[ Splash ]")]
        [SerializeField] CanvasGroup splashPanel;

        public override async UniTask ShowAsync(Action onComplete = null)
        {
            VisibleState = VisibleState.Appearing;
            gameObject.SetActive(true);
            VisibleState = VisibleState.Appeared;
            onComplete?.Invoke();
            await UniTask.CompletedTask;
        }

        public override void ShowImmediate()
        {
            VisibleState = VisibleState.Appearing;
            gameObject.SetActive(true);
            VisibleState = VisibleState.Appeared;
        }

        public override async UniTask HideAsync(Action onComplete = null)
        {
            VisibleState = VisibleState.Disappearing;
            gameObject.SetActive(false);
            VisibleState = VisibleState.Disappeared;
            onComplete?.Invoke();
            await UniTask.CompletedTask;
        }

        public override void HideImmediate()
        {
            VisibleState = VisibleState.Disappearing;
            gameObject.SetActive(false);
            VisibleState = VisibleState.Disappeared;
        }

        public async UniTask FadeInAsync(float duration = 0.8f)
        {
            gameObject.SetActive(true);
            await FadeAsync(0f, 1f, duration);
        }

        public async UniTask FadeOutAsync(float duration = 0.6f)
        {
            await FadeAsync(1f, 0f, duration);
            gameObject.SetActive(false);
        }

        async UniTask FadeAsync(float from, float to, float duration)
        {
            if (splashPanel == null) return;
            float elapsed = 0f;
            splashPanel.alpha = from;
            while (elapsed < duration)
            {
                elapsed           += Time.deltaTime;
                splashPanel.alpha  = Mathf.Lerp(from, to, elapsed / duration);
                await UniTask.Yield(PlayerLoopTiming.Update);
            }
            splashPanel.alpha = to;
        }
    }
}
