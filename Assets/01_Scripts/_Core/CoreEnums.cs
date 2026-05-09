namespace RottenNoble.Core
{
    /// <summary>
    /// UI View 가시성 상태
    /// </summary>
    public enum VisibleState
    {
        None,

        Appearing,
        Appeared,

        Disappearing,
        Disappeared,
    }

    /// <summary>
    /// Canvas 레이어 타입 — Sort Order 낮을수록 아래 렌더링
    ///
    ///  Hud(100) → Popup(200) → Overlay(300)
    ///
    ///  Hud     : 인게임 HUD(소울 게이지·파티 상태), 메인메뉴 탭 UI
    ///  Popup   : 다이얼로그, 설정창, 아이템 상세 등 모달 UI
    ///  Overlay : 씬 전환 페이드, 로딩 화면 등 전체 화면 덮는 최상위 레이어
    /// </summary>
    public enum CanvasType
    {
        Hud     = 0,
        Popup   = 1,
        Overlay = 2,
    }

    /// <summary>
    /// 리소스 로드 방식 구분
    /// </summary>
    public enum ResourceType
    {
        /// <summary>Resources 폴더 로컬 에셋</summary>
        Resource,

        /// <summary>Addressables 원격/로컬 에셋</summary>
        Addressable,
    }
}
