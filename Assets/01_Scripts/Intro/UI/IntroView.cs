using UnityEngine;
using TMPro;

using Cysharp.Threading.Tasks;

using RottenNoble.Core.UI;

namespace RottenNoble.Intro.UI
{
    /// <summary>
    /// Intro 씬 View — 타이틀 표시 + 클릭 유도 텍스트
    /// 페이드인으로 등장, 클릭 후 페이드아웃으로 퇴장
    /// </summary>
    public class IntroView : ViewBase
    {
        [Header("[ Intro ]")]
        [SerializeField] CanvasGroup canvasGroup;
        [SerializeField] TMP_Text    titleText;
        [SerializeField] TMP_Text    promptText;

        [Header("[ Fade ]")]
        [SerializeField] float fadeInDuration  = 1.0f;
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
            if (canvasGroup != null) canvasGroup.alpha = 1f;
        }

        protected override void OnHideImmediate()
        {
            if (canvasGroup != null) canvasGroup.alpha = 0f;
            gameObject.SetActive(false);
        }

        // ── Helper ────────────────────────────────────────────────────────

        async UniTask FadeAsync(float from, float to, float duration)
        {
            if (canvasGroup == null) return;

            float elapsed = 0f;
            canvasGroup.alpha = from;

            while (elapsed < duration)
            {
                elapsed           += Time.deltaTime;
                canvasGroup.alpha  = Mathf.Lerp(from, to, elapsed / duration);
                await UniTask.Yield(PlayerLoopTiming.Update);
            }

            canvasGroup.alpha = to;
        }
    }
}
