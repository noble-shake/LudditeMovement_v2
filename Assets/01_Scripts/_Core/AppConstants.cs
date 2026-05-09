using System;

namespace RottenNoble.Core
{
    /// <summary>
    /// 앱 전역 상수 모음
    /// </summary>
    public static class AppConstants
    {
        // ── UI 스로틀 ─────────────────────────────────────
        public static readonly TimeSpan ButtonThrottle = TimeSpan.FromSeconds(0.3f);
        public static readonly TimeSpan SliderThrottle = TimeSpan.FromSeconds(0.02f);

        // ── 씬 전환 ───────────────────────────────────────
        public const float SceneFadeDuration = 0.4f;

        // ── 소울 경제 ─────────────────────────────────────
        public const float SoulDrainBaseRate = 1f;
        public const float SoulMaxCapacity   = 100f;

        // ── InGame ────────────────────────────────────────
        public const int MaxPartySize = 4;
    }
}
