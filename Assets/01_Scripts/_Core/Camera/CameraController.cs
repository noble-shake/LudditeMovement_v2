using System;
using UnityEngine;

using R3;
using VContainer;

using RottenNoble.Core.Input;

namespace RottenNoble.Core
{
    public enum CameraMode
    {
        Fixed,   // 고정 (Splash, Intro 등)
        MapPan,  // 마우스 드래그 패닝 (MainMenu 맵)
        Follow,  // 타겟 추적 (InGame)
    }

    /// <summary>
    /// 메인 카메라 모드 관리자.
    ///
    ///   Fixed   — 카메라 정지
    ///   MapPan  — 마우스 드래그로 맵 패닝 (경계 클램프)
    ///   Follow  — Transform 추적 (부드러운 보간)
    ///
    /// EntryPoint에서 씬 진입 시 SetMode() 호출로 전환
    /// </summary>
    public class CameraController : IDisposable
    {
        readonly InputManager inputManager;

        Camera    mainCamera;
        Bounds    mapBounds;
        Transform followTarget;

        readonly CompositeDisposable modeDisposables = new();
        readonly CompositeDisposable disposables     = new();

        [Inject]
        public CameraController(InputManager inputManager)
        {
            this.inputManager = inputManager;
        }

        // ── 공개 API ──────────────────────────────────────────────

        /// <summary>카메라 모드 전환. 이전 모드 구독 자동 해제.</summary>
        public void SetMode(CameraMode mode)
        {
            modeDisposables.Clear();

            if (mainCamera == null)
                mainCamera = Camera.main;

            switch (mode)
            {
                case CameraMode.MapPan: StartMapPan(); break;
                case CameraMode.Follow: StartFollow(); break;
            }
        }

        /// <summary>MapPan 모드 이동 경계 설정.</summary>
        public void SetMapBounds(Bounds bounds) => mapBounds = bounds;

        /// <summary>Follow 타겟 지정 후 Follow 모드 전환.</summary>
        public void Follow(Transform target)
        {
            followTarget = target;
            SetMode(CameraMode.Follow);
        }

        /// <summary>카메라를 월드 좌표로 즉시 이동 (z축 유지).</summary>
        public void Snap(Vector3 worldPos)
        {
            if (mainCamera == null) return;
            mainCamera.transform.position = new Vector3(
                worldPos.x,
                worldPos.y,
                mainCamera.transform.position.z);
        }

        // ── MapPan ────────────────────────────────────────────────

        void StartMapPan()
        {
            inputManager.MouseDelta
                .WithLatestFrom(inputManager.LeftClick, (delta, held) => (delta, held))
                .Where(x => x.held)
                .Subscribe(x => ApplyPan(x.delta))
                .AddTo(modeDisposables);
        }

        void ApplyPan(Vector2 screenDelta)
        {
            // 스크린 픽셀 → 월드 단위 변환 (직교 카메라 기준)
            float unitsPerPixel = mainCamera.orthographicSize * 2f / Screen.height;
            Vector3 move = new Vector3(-screenDelta.x, -screenDelta.y, 0f) * unitsPerPixel;
            Vector3 next = mainCamera.transform.position + move;

            // 맵 경계 클램프
            if (mapBounds.size != Vector3.zero)
            {
                next.x = Mathf.Clamp(next.x, mapBounds.min.x, mapBounds.max.x);
                next.y = Mathf.Clamp(next.y, mapBounds.min.y, mapBounds.max.y);
            }

            mainCamera.transform.position = next;
        }

        // ── Follow ────────────────────────────────────────────────

        void StartFollow()
        {
            Observable.EveryUpdate()
                .Subscribe(_ => ApplyFollow())
                .AddTo(modeDisposables);
        }

        void ApplyFollow()
        {
            if (followTarget == null || mainCamera == null) return;

            Vector3 target = followTarget.position;
            target.z = mainCamera.transform.position.z;
            mainCamera.transform.position = Vector3.Lerp(
                mainCamera.transform.position, target, Time.deltaTime * 5f);
        }

        // ── IDisposable ───────────────────────────────────────────

        public void Dispose()
        {
            modeDisposables.Dispose();
            disposables.Dispose();
        }
    }
}
