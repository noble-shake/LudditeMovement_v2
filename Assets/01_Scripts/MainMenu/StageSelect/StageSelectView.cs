using UnityEngine.UI;

using R3;

using RottenNoble.Core;
using RottenNoble.Core.UI;

namespace RottenNoble.MainMenu.StageSelect
{
    /// <summary>
    /// 스테이지 선택 탭 View — 스테이지 목록 / 파티 구성 / 스킬 슬롯 UI 담당
    /// Show/Hide 는 ViewBase 기본 동작(SetActive) 사용
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
    }
}
