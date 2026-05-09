using UnityEngine;

namespace RottenNoble.Core.UI
{
    /// <summary>
    /// 디바이스 SafeArea에 맞춰 RectTransform 앵커를 자동 조정합니다.
    ///
    /// [타겟 전제]
    /// - 기본: PC (safeArea == 전체 화면, 사실상 아무것도 하지 않음)
    /// - 모바일: 가로 모드(Landscape) 전용
    /// - 노치 위치: 오른쪽 (사이드바/뒤로가기 버튼이 왼쪽에 배치)
    ///
    /// Screen.safeArea가 오른쪽 노치를 자동으로 반영하므로
    /// (anchorMax.x < 1) 별도 처리 없이 올바르게 동작합니다.
    /// 좌우 Padding을 개별 설정하여 디자인 여백을 추가할 수 있습니다.
    /// </summary>
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    public class SafeAreaRectTransform : MonoBehaviour
    {
        [Header("[ Extra Padding (Canvas Units) ]")]
        [Tooltip("SafeArea 위에 추가로 적용할 왼쪽 여백")]
        public float leftPadding   = 0f;
        [Tooltip("SafeArea 위에 추가로 적용할 오른쪽 여백 (노치 쪽 추가 여백)")]
        public float rightPadding  = 0f;
        [Tooltip("SafeArea 위에 추가로 적용할 위쪽 여백")]
        public float topPadding    = 0f;
        [Tooltip("SafeArea 위에 추가로 적용할 아래쪽 여백")]
        public float bottomPadding = 0f;

#if UNITY_EDITOR
        [Header("[ Simulate Safe Area (Editor Only) ]")]
        [Tooltip("에디터에서 SafeArea 시뮬레이션 활성화")]
        public bool useSimulatedSafeArea = false;

        [Tooltip("가로 모드 기준. 오른쪽 노치 시뮬레이션 예시: (0, 0, 1820, 1080) — 오른쪽 100px 컷")]
        public Rect simulatedSafeArea = new Rect(0, 0, 1920, 1080);
#endif

        [Header("[ Debug (ReadOnly) ]")]
        public Rect    debugSafeArea;
        public Vector2 debugAnchorMin;
        public Vector2 debugAnchorMax;

        RectTransform     _rect;
        Rect              _lastSafeArea    = Rect.zero;
        Vector2Int        _lastScreenSize  = Vector2Int.zero;
        ScreenOrientation _lastOrientation = ScreenOrientation.Unknown;
        float _lastLeft, _lastRight, _lastTop, _lastBottom;
#if UNITY_EDITOR
        bool _lastUseSimulated;
        Rect _lastSimulated = Rect.zero;
#endif

        void Awake()
        {
            _rect = GetComponent<RectTransform>();
            ApplySafeArea();
        }

        void Update()
        {
            if (NeedsRefresh()) ApplySafeArea();
        }

        bool NeedsRefresh()
        {
            if (_lastSafeArea != Screen.safeArea)                                           return true;
            if (_lastScreenSize.x != Screen.width || _lastScreenSize.y != Screen.height)   return true;
            if (_lastOrientation != Screen.orientation)                                     return true;
            if (_lastLeft != leftPadding || _lastRight != rightPadding)                     return true;
            if (_lastTop  != topPadding  || _lastBottom != bottomPadding)                   return true;
#if UNITY_EDITOR
            if (_lastUseSimulated != useSimulatedSafeArea)                                  return true;
            if (useSimulatedSafeArea && _lastSimulated != simulatedSafeArea)                return true;
#endif
            return false;
        }

        public void ApplySafeArea()
        {
            if (_rect == null) _rect = GetComponent<RectTransform>();

#if UNITY_EDITOR
            Rect area         = useSimulatedSafeArea ? simulatedSafeArea : Screen.safeArea;
            _lastUseSimulated = useSimulatedSafeArea;
            _lastSimulated    = simulatedSafeArea;
#else
            Rect area = Screen.safeArea;
#endif
            float sw = Screen.width;
            float sh = Screen.height;
            if (sw == 0 || sh == 0) return;

            _lastSafeArea      = Screen.safeArea;
            _lastScreenSize    = new Vector2Int(Screen.width, Screen.height);
            _lastOrientation   = Screen.orientation;
            _lastLeft   = leftPadding;  _lastRight  = rightPadding;
            _lastTop    = topPadding;   _lastBottom = bottomPadding;

            // SafeArea → 정규화 앵커
            Vector2 anchorMin = new Vector2(area.x / sw,          area.y / sh);
            Vector2 anchorMax = new Vector2((area.x + area.width) / sw, (area.y + area.height) / sh);

            _rect.anchorMin = anchorMin;
            _rect.anchorMax = anchorMax;

            // 추가 패딩 (offset 방식)
            _rect.offsetMin = new Vector2(leftPadding,   bottomPadding);
            _rect.offsetMax = new Vector2(-rightPadding, -topPadding);

            debugSafeArea  = area;
            debugAnchorMin = anchorMin;
            debugAnchorMax = anchorMax;
        }
    }
}
