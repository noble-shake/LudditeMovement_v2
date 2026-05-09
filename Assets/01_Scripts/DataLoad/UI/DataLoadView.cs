using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Cysharp.Threading.Tasks;

using RottenNoble.Core;
using RottenNoble.Core.UI;

namespace RottenNoble.DataLoad.UI
{
    /// <summary>
    /// DataLoad 씬 View — 리소스 로딩 진행률 표시 UI
    /// </summary>
    public class DataLoadView : ViewBase
    {
        [Header("[ Progress ]")]
        [SerializeField] Slider   progressBar;
        [SerializeField] TMP_Text progressText;
        [SerializeField] TMP_Text statusText;

        public void SetProgress(float value)
        {
            if (progressBar  != null) progressBar.value = value;
            if (progressText != null) progressText.text = $"{(int)(value * 100)}%";
        }

        public void SetStatus(string status)
        {
            if (statusText != null) statusText.text = status;
        }

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
    }
}
