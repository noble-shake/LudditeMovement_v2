using System;
using UnityEngine;
using UnityEngine.UI;

using Cysharp.Threading.Tasks;
using R3;

using RottenNoble.Core;
using RottenNoble.Core.UI;
using TMPro;

namespace RottenNoble.Intro.UI
{
    /// <summary>
    /// Intro 씬 View — 게임 타이틀 및 "게임 시작" 버튼 담당
    /// </summary>
    public class IntroView : ViewBase
    {
        [Header("[ Intro ]")]
        [field : SerializeField] public TMP_Text IntroText {get; set;}
        

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
