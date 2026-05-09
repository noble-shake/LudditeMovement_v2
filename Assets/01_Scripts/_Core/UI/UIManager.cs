using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

using RottenNoble.Core.UI;

namespace RottenNoble.Core
{
    /// <summary>
    /// UI Canvas 레이어 관리자.
    ///
    /// ┌──────────────────────────────────────────────────────┐
    /// │  Sort  │  Layer   │  용도                            │
    /// ├──────────────────────────────────────────────────────┤
    /// │   300  │  Overlay │  씬 전환 페이드, 로딩, 풀스크린  │
    /// │   200  │  Popup   │  다이얼로그, 설정창, 모달 UI     │
    /// │   100  │  Hud     │  인게임 HUD, 메인메뉴 기본 UI   │
    /// └──────────────────────────────────────────────────────┘
    ///
    /// - Bootstrap 씬의 AppLifetimeScope 하위에 배치 → DontDestroyOnLoad
    /// - 각 Canvas에 CanvasScaler(1920×1080, MatchHeight) 자동 적용
    /// - VContainer로 Singleton 주입
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        // ── Inspector ────────────────────────────────────────────
        [Header("[ Canvas Layers ]")]
        [SerializeField] CanvasSet _hud;
        [SerializeField] CanvasSet _popup;
        [SerializeField] CanvasSet _overlay;

        [Header("[ Scale Settings ]")]
        [SerializeField] Vector2 _referenceResolution = new(1920f, 1080f);

        // 가로 전용 게임 → Height 기준(1.0) 고정
        // 화면이 넓어지면 가로 공간만 늘어나고 세로 레이아웃은 유지됩니다.
        [SerializeField][Range(0f, 1f)] float _matchWidthOrHeight = 1f;

        // ── Sort Order ───────────────────────────────────────────
        const int SortHud     = 100;
        const int SortPopup   = 200;
        const int SortOverlay = 300;

        // ── 내부 맵 ──────────────────────────────────────────────
        Dictionary<CanvasType, CanvasSet> _map;

        // ── 초기화 ────────────────────────────────────────────────
        void Awake()
        {
            DontDestroyOnLoad(gameObject);

            _map = new Dictionary<CanvasType, CanvasSet>
            {
                { CanvasType.Hud,     _hud     },
                { CanvasType.Popup,   _popup   },
                { CanvasType.Overlay, _overlay },
            };

            SetupCanvas(_hud,     SortHud);
            SetupCanvas(_popup,   SortPopup);
            SetupCanvas(_overlay, SortOverlay);
        }

        void SetupCanvas(CanvasSet set, int sortOrder)
        {
            if (set?.Canvas == null) return;

            // Sort Order
            set.Canvas.sortingOrder = sortOrder;

            // CanvasScaler
            var scaler = set.Canvas.GetComponent<CanvasScaler>()
                      ?? set.Canvas.gameObject.AddComponent<CanvasScaler>();

            scaler.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = _referenceResolution;
            scaler.screenMatchMode     = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight  = _matchWidthOrHeight;
        }

        // ── Canvas 접근 ───────────────────────────────────────────
        public Canvas    GetCanvas(CanvasType type)          => _map[type]?.Canvas;
        public Transform GetCanvasTransform(CanvasType type) => GetCanvas(type)?.transform;
        public Transform GetSafeAreaTransform(CanvasType type) => _map[type]?.SafeAreaContainer;

        // ── 배치 ──────────────────────────────────────────────────
        /// <summary>uiRoot를 해당 레이어의 SafeArea 하위에 배치합니다.</summary>
        public void Attach(CanvasType type, GameObject uiRoot)
        {
            var parent = GetSafeAreaTransform(type) ?? GetCanvasTransform(type);
            if (parent != null)
                uiRoot.transform.SetParent(parent, false);
        }

        // ── View 검색 ──────────────────────────────────────────────
        public T GetView<T>(CanvasType type) where T : ViewBase
            => GetCanvas(type)?.GetComponentsInChildren<T>(true).FirstOrDefault();

        public T GetView<T>(CanvasType type, string objectName) where T : ViewBase
            => GetCanvas(type)
                ?.GetComponentsInChildren<ViewBase>(true)
                .FirstOrDefault(v => v.gameObject.name == objectName) as T;

        // ── Popup 레이어 활성/비활성 ──────────────────────────────
        /// <summary>Popup Canvas 전체를 켜거나 끕니다.</summary>
        public void SetPopupActive(bool active)
        {
            if (_popup?.Canvas != null)
                _popup.Canvas.gameObject.SetActive(active);
        }

        // ── 정리 ──────────────────────────────────────────────────
        public void ClearCanvas(CanvasType type)
        {
            var safeArea = GetSafeAreaTransform(type);
            if (safeArea == null) return;

            foreach (Transform child in safeArea)
                Destroy(child.gameObject);
        }

        public void ClearAll()
        {
            foreach (var type in _map.Keys)
                ClearCanvas(type);
        }
    }

    // ── CanvasSet ─────────────────────────────────────────────────
    [Serializable]
    public class CanvasSet
    {
        [field: SerializeField] public Canvas    Canvas           { get; private set; }

        /// <summary>
        /// SafeArea 컨테이너. SafeAreaRectTransform 컴포넌트를 붙인 자식 오브젝트를 연결합니다.
        /// Overlay처럼 전체 화면을 덮는 레이어는 비워둡니다.
        /// </summary>
        [field: SerializeField] public Transform SafeAreaContainer { get; private set; }

        // CanvasType은 UIManager에서 직접 매핑하므로 CanvasSet에서 제거
        public static implicit operator bool(CanvasSet c) => c?.Canvas != null;
    }
}
