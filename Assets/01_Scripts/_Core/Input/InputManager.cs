using System;
using UnityEngine;
using UnityEngine.InputSystem;

using R3;

namespace RottenNoble.Core.Input
{
    /// <summary>
    /// SoulHeroes 전용 마우스 입력 관리자.
    ///
    /// ┌─────────────────────────────────────────────────────────────────┐
    /// │  SoulHeroes는 마우스 특화 게임입니다.                           │
    /// │  커서는 항상 표시 상태(Visible + None)를 유지합니다.           │
    /// │                                                                 │
    /// │  InputMode.Game  — 게임 클릭/드래그/스크롤 활성화             │
    /// │  InputMode.UI    — 게임 클릭 비활성화, EventSystem이 처리     │
    /// │                    (커서 위치/델타는 항상 추적)                │
    /// └─────────────────────────────────────────────────────────────────┘
    ///
    /// AppLifetimeScope에 Singleton 등록 →  VContainer로 전역 주입
    /// </summary>
    public class InputManager : IDisposable
    {
        // ── 마우스 위치 ────────────────────────────────────────────────────

        /// <summary>현재 마우스 스크린 좌표 (PointerOrb 추적, 히어로 클릭 판정)</summary>
        public ReadOnlyReactiveProperty<Vector2> MousePosition => mousePosition;
        readonly ReactiveProperty<Vector2> mousePosition = new(Vector2.zero);

        /// <summary>프레임 단위 마우스 이동 델타 (PointerOrb 회전 방향 감지)</summary>
        public ReadOnlyReactiveProperty<Vector2> MouseDelta => mouseDelta;
        readonly ReactiveProperty<Vector2> mouseDelta = new(Vector2.zero);

        // ── 마우스 버튼 ────────────────────────────────────────────────────

        /// <summary>좌클릭 누름 상태 (영웅 드래그 시작/종료, 환경 상호작용)</summary>
        public ReadOnlyReactiveProperty<bool> LeftClick => leftClick;
        readonly ReactiveProperty<bool> leftClick = new(false);

        /// <summary>우클릭 누름 상태 (대안 조작 또는 취소)</summary>
        public ReadOnlyReactiveProperty<bool> RightClick => rightClick;
        readonly ReactiveProperty<bool> rightClick = new(false);

        /// <summary>가운데 클릭 누름 상태</summary>
        public ReadOnlyReactiveProperty<bool> MiddleClick => middleClick;
        readonly ReactiveProperty<bool> middleClick = new(false);

        // ── 스크롤 ─────────────────────────────────────────────────────────

        /// <summary>스크롤 휠 입력 (y 값 사용, 양수 = 위, 음수 = 아래)</summary>
        public ReadOnlyReactiveProperty<Vector2> ScrollWheel => scrollWheel;
        readonly ReactiveProperty<Vector2> scrollWheel = new(Vector2.zero);

        // ── InputActions ───────────────────────────────────────────────────

        readonly InputAction positionAction;
        readonly InputAction deltaAction;
        readonly InputAction leftClickAction;
        readonly InputAction rightClickAction;
        readonly InputAction middleClickAction;
        readonly InputAction scrollAction;

        DisposableBag disposables;
        bool disposed;

        // ── 생성자 ─────────────────────────────────────────────────────────

        public InputManager()
        {
            // 마우스 전용 Actions 생성
            positionAction    = new InputAction("MousePosition", InputActionType.Value,   "<Mouse>/position");
            deltaAction       = new InputAction("MouseDelta",    InputActionType.Value,   "<Mouse>/delta");
            leftClickAction   = new InputAction("LeftClick",     InputActionType.Button,  "<Mouse>/leftButton");
            rightClickAction  = new InputAction("RightClick",    InputActionType.Button,  "<Mouse>/rightButton");
            middleClickAction = new InputAction("MiddleClick",   InputActionType.Button,  "<Mouse>/middleButton");
            scrollAction      = new InputAction("ScrollWheel",   InputActionType.Value,   "<Mouse>/scroll");

            BindObservables();

            // SoulHeroes: 커서 항상 표시
            Cursor.visible   = true;
            Cursor.lockState = CursorLockMode.None;

            // 초기 모드: UI (씬 로드 시작 시점)
            SetInputMode(InputMode.UI);
        }

        // ── 모드 전환 ──────────────────────────────────────────────────────

        public enum InputMode
        {
            /// <summary>InGame 플레이 중 — 클릭·드래그·스크롤 모두 활성화</summary>
            Game,
            /// <summary>메뉴·로딩 중 — 게임 입력 비활성화, EventSystem이 클릭 처리</summary>
            UI,
        }

        public void SetInputMode(InputMode mode)
        {
            // 커서는 항상 표시 (모드 전환과 무관)
            Cursor.visible   = true;
            Cursor.lockState = CursorLockMode.None;

            // 위치·델타는 모드와 관계없이 항상 추적
            positionAction.Enable();
            deltaAction.Enable();

            if (mode == InputMode.Game)
            {
                leftClickAction.Enable();
                rightClickAction.Enable();
                middleClickAction.Enable();
                scrollAction.Enable();
            }
            else // UI: 게임 클릭은 비활성화, EventSystem이 담당
            {
                leftClickAction.Disable();
                rightClickAction.Disable();
                middleClickAction.Disable();
                scrollAction.Disable();

                // 상태 초기화
                leftClick.Value   = false;
                rightClick.Value  = false;
                middleClick.Value = false;
                scrollWheel.Value = Vector2.zero;
            }
        }

        // ── Dispose ────────────────────────────────────────────────────────

        public void Dispose()
        {
            if (disposed) return;
            disposed = true;

            disposables.Dispose();

            positionAction?.Dispose();
            deltaAction?.Dispose();
            leftClickAction?.Dispose();
            rightClickAction?.Dispose();
            middleClickAction?.Dispose();
            scrollAction?.Dispose();

            mousePosition.Dispose();
            mouseDelta.Dispose();
            leftClick.Dispose();
            rightClick.Dispose();
            middleClick.Dispose();
            scrollWheel.Dispose();
        }

        // ── 내부 ───────────────────────────────────────────────────────────

        void BindObservables()
        {
            // 위치: performed 이벤트로 지속 갱신
            positionAction.PerformedValueAsObservable<Vector2>()
                .Subscribe(v => mousePosition.Value = v)
                .AddTo(ref disposables);

            // 델타: performed 갱신 + canceled 시 0 리셋 (프레임 끝)
            deltaAction.PerformedValueAsObservable<Vector2>()
                .Subscribe(v => mouseDelta.Value = v)
                .AddTo(ref disposables);

            deltaAction.CanceledAsObservable()
                .Subscribe(_ => mouseDelta.Value = Vector2.zero)
                .AddTo(ref disposables);

            // 버튼: performed=true / canceled=false
            leftClickAction.AsBoolObservable()
                .Subscribe(v => leftClick.Value = v)
                .AddTo(ref disposables);

            rightClickAction.AsBoolObservable()
                .Subscribe(v => rightClick.Value = v)
                .AddTo(ref disposables);

            middleClickAction.AsBoolObservable()
                .Subscribe(v => middleClick.Value = v)
                .AddTo(ref disposables);

            // 스크롤: performed 갱신 + canceled 시 0 리셋
            scrollAction.PerformedValueAsObservable<Vector2>()
                .Subscribe(v => scrollWheel.Value = v)
                .AddTo(ref disposables);

            scrollAction.CanceledAsObservable()
                .Subscribe(_ => scrollWheel.Value = Vector2.zero)
                .AddTo(ref disposables);
        }
    }
}
