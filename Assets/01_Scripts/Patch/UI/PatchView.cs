using UnityEngine.UI;
using TMPro;

using RottenNoble.Core.UI;

namespace RottenNoble.Patch.UI
{
    /// <summary>
    /// Patch 씬 View — 업데이트 진행률 표시 UI
    /// 애니메이션 없음 — ViewBase 기본 Show/Hide 동작 사용
    /// </summary>
    public class PatchView : ViewBase
    {
        [UnityEngine.Header("[ Progress ]")]
        [UnityEngine.SerializeField] Slider   progressBar;
        [UnityEngine.SerializeField] TMP_Text progressText;
        [UnityEngine.SerializeField] TMP_Text statusText;

        public void SetProgress(float value)
        {
            if (progressBar  != null) progressBar.value = value;
            if (progressText != null) progressText.text = $"{(int)(value * 100)}%";
        }

        public void SetStatus(string status)
        {
            if (statusText != null) statusText.text = status;
        }
    }
}
