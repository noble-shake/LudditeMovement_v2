namespace RottenNoble.Core
{
    /// <summary>
    /// 앱 전체 PlayerPrefs 키 목록.
    /// 문자열 직접 사용 금지 — 항상 이 enum 경유.
    /// </summary>
    public enum PlayerPrefKey
    {
        // ── Gameplay ─────────────────────────────────
        Difficulty,         // DifficultyLevel (int)

        // ── Audio ─────────────────────────────────────
        MasterVolume,       // float  0~1
        BGMVolume,          // float  0~1
        SFXVolume,          // float  0~1

        // ── Display ───────────────────────────────────
        FullScreen,         // bool   (int 0/1)
        ResolutionIndex,    // int    해상도 프리셋 인덱스

        // ── 기타 ──────────────────────────────────────
        // 새 설정 추가 시 여기에만 추가
    }
}
