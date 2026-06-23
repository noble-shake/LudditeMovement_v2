using UnityEngine;

namespace RottenNoble.Core
{
    /// <summary>
    /// 난이도별 색상 테마 ScriptableObject.
    ///
    /// Normal  — 앰버/주황
    /// Hard    — 레드/선홍
    /// Lunatic — 퍼플/검붉은
    ///
    /// 탄막 색상은 스프라이트 자체 관리 — 여기 포함하지 않음.
    /// ColorManager.CurrentTheme 을 구독해 UI·환경·포스트프로세싱에 적용.
    /// </summary>
    [CreateAssetMenu(fileName = "Theme_Normal", menuName = "RottenNoble/Config/Difficulty Theme")]
    public class DifficultyThemeSO : ScriptableObject
    {
        [Header("[ Identity ]")]
        public DifficultyLevel level;

        [Header("[ UI ]")]
        [Tooltip("버튼 테두리, 강조 텍스트 등 주 포인트 색상")]
        public Color primaryColor   = Color.white;

        [Tooltip("서브 강조, 아이콘 색조")]
        public Color secondaryColor = Color.white;

        [Tooltip("UI 패널 배경 색조 (반투명 적용 시 기준색)")]
        public Color uiPanelTint    = Color.black;

        [Header("[ Environment ]")]
        [Tooltip("씬 배경 전체 색조")]
        public Color backgroundTint = Color.black;

        [Tooltip("안개, 원거리 아우라 색상")]
        public Color fogColor       = Color.black;

        [Header("[ Post Processing ]")]
        [Tooltip("화면 외곽 비네트 색상")]
        public Color  vignetteColor     = Color.black;

        [Tooltip("비네트 강도 (0 = 없음, 1 = 최대)")]
        [Range(0f, 1f)]
        public float  vignetteIntensity = 0.3f;
    }
}
