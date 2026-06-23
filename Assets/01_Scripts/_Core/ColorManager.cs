using System;
using System.Collections.Generic;
using R3;
using VContainer;

namespace RottenNoble.Core
{
    /// <summary>
    /// 난이도 테마 3종 묶음.
    /// VContainer가 같은 타입(DifficultyThemeSO) 3개를 구분할 수 없으므로
    /// 이 래퍼를 RegisterInstance로 등록 → ColorManager에 주입.
    /// </summary>
    public class DifficultyThemePack
    {
        public DifficultyThemeSO Normal  { get; }
        public DifficultyThemeSO Hard    { get; }
        public DifficultyThemeSO Lunatic { get; }

        public DifficultyThemePack(
            DifficultyThemeSO normal,
            DifficultyThemeSO hard,
            DifficultyThemeSO lunatic)
        {
            Normal  = normal;
            Hard    = hard;
            Lunatic = lunatic;
        }
    }

    /// <summary>
    /// 난이도 기반 색상 테마 관리자.
    ///
    /// SetDifficulty() 로 테마 전환 →
    /// CurrentTheme 구독자(UI, 환경, 포스트프로세싱)가 자동 반응.
    ///
    /// 탄막 색상은 스프라이트 자체 관리 — ColorManager 범위 아님.
    ///
    /// 사용 예시:
    ///   dataManager.Color.SetDifficulty(DifficultyLevel.Hard);
    ///   dataManager.Color.CurrentTheme.Subscribe(t => ApplyTheme(t)).AddTo(ref bag);
    /// </summary>
    public class ColorManager : IDisposable
    {
        readonly Dictionary<DifficultyLevel, DifficultyThemeSO> themes = new();
        readonly ReactiveProperty<DifficultyThemeSO> currentTheme      = new();
        readonly CompositeDisposable disposables                        = new();

        public ReadOnlyReactiveProperty<DifficultyThemeSO> CurrentTheme => currentTheme;
        public DifficultyLevel CurrentLevel { get; private set; } = DifficultyLevel.Normal;

        [Inject]
        public ColorManager(DifficultyThemePack pack)
        {
            themes[DifficultyLevel.Normal]  = pack.Normal;
            themes[DifficultyLevel.Hard]    = pack.Hard;
            themes[DifficultyLevel.Lunatic] = pack.Lunatic;

            currentTheme.Value = pack.Normal;
        }

        // ── 공개 API ──────────────────────────────────────────────

        /// <summary>난이도 테마 전환. CurrentTheme 구독자에게 즉시 전파.</summary>
        public void SetDifficulty(DifficultyLevel level)
        {
            if (!themes.TryGetValue(level, out var theme)) return;
            CurrentLevel       = level;
            currentTheme.Value = theme;
        }

        /// <summary>특정 난이도 테마 직접 접근 (구독 없이 즉시 읽기용).</summary>
        public DifficultyThemeSO GetTheme(DifficultyLevel level)
            => themes.TryGetValue(level, out var t) ? t : null;

        // ── IDisposable ───────────────────────────────────────────

        public void Dispose()
        {
            currentTheme.Dispose();
            disposables.Dispose();
        }
    }
}
