using System;

using Cysharp.Threading.Tasks;

using RottenNoble.Core;
using RottenNoble.Core.UI;

namespace RottenNoble.MainMenu.Collection
{
    /// <summary>
    /// 도감 탭 View — 몬스터 로어 / 영웅 스토리 조각 / 친밀도 확인 UI 담당
    /// </summary>
    public class CollectionView : ViewBase
    {
        // TODO: 서브탭 버튼 (Monster/HeroStory/Affinity), 아이템 리스트 UI 추가

        // ── ViewBase 구현 ──────────────────────────────────
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
