using System;

using Cysharp.Threading.Tasks;

using RottenNoble.Core;
using RottenNoble.Core.UI;

namespace RottenNoble.MainMenu.Character
{
    /// <summary>
    /// 캐릭터 탭 View — 영웅 정보 / 스킬 트리 확인 UI 담당
    /// </summary>
    public class CharacterView : ViewBase
    {
        // TODO: 영웅 선택 버튼, 스킬 트리 노드 UI 레퍼런스 추가

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
