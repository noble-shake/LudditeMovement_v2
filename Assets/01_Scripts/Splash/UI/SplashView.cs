using UnityEngine;

using Cysharp.Threading.Tasks;

using RottenNoble.Core.UI;

namespace RottenNoble.Splash.UI
{
    /// <summary>
    /// Splash 씬 View — 로고 패널 페이드인 / 페이드아웃 담당
    /// </summary>
    public class SplashView : ViewBase
    {
        [Header("[ Splash ]")]
        [SerializeField] CanvasGroup splashPanel;

        [Header("[ Fade ]")]
        [SerializeField] float fadeInDuration  = 0.8f;
        [SerializeField] float fadeOutDuration = 0.6f;

        protected override async UniTask OnShowAsync()
        {
            gameObject.SetActive(true);
            await FadeAsync(0f, 1f, fadeInDuration);
        }

        protected override async UniTask OnHideAsync()
        {
            await FadeAsync(1f, 0f, fadeOutDuration);
            gameObject.SetActive(false);
        }

        protected override void OnShowImmediate()
        {
            gameObject.SetActive(true);
            if (splashPanel != null) splashPanel.alpha = 1f;
        }

        protected override void OnHideImmediate()
        {
            if (splashPanel != null) splashPanel.alpha = 0f;
            gameObject.SetActive(false);
        }

        // ── Helper ────────────────────────────────────────────────────────

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
