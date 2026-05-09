using System;
using UnityEngine.UI;

using Cysharp.Threading.Tasks;
using R3;

using RottenNoble.Core;
using RottenNoble.Core.UI;

namespace RottenNoble.MainMenu.StageSelect
{
    /// <summary>
    /// 스테이지 선택 탭 View — 스테이지 목록 / 파티 구성 / 스킬 슬롯 UI 담당
    /// </summary>
    public class StageSelectView : ViewBase
    {
        [UnityEngine.Header("[ Stage Select ]")]
        [UnityEngine.SerializeField] Button startButton;

        // TODO: 영웅 버튼 그리드, 스테이지 셀 목록, 스킬 슬롯 UI 레퍼런스 추가

        public Observable<Unit> OnStartButtonClicked()
            => startButton.OnClickAsObservable()
                .ThrottleFirst(AppConstants.ButtonThrottle).Share();

        public void SetStartButtonInteractable(bool interactable)
        {
            if (startButton != null) startButton.interactable = interactable;
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
