# SoulHeroes — Full Context

> 코드 아키텍처 + 디자인 방향 통합 컨텍스트.
> Claude Code 세션 / Figma AI / 협업 도구에 공유용.

---

## 프로젝트 개요

- **게임**: 탄막 슈팅 + 실시간 유닛 관리 하이브리드
- **플랫폼**: PC (Steam 출시 목표), 마우스 특화 조작
- **개발 형태**: 솔로 인디
- **레포**: `github.com/noble-shake/LudditeMovement_v2` / 브랜치 `feature/renewal`
- **경로**: `E:\workspace\SoulHeroes`
- **엔진**: Unity 6 / C#
- **해상도**: 1920×1080 (16:9) FHD 기준

---

## 세계관

대부분의 생명체가 죽고 몬스터화된 세계.
거대한 성이 괴물들에게 강탈당했으며,
플레이어는 **6인의 용사 영혼**을 이끌고 성을 탈환하는 여정을 이끈다.

- **주체**: 영혼(Soul) — 육체 없이 의지만 남은 존재
- **적**: 몬스터화된 생명체들, 성을 점령한 존재들
- **배경**: 폐허가 된 성과 그 일대의 지역들

---

## 아트 스타일

### 레퍼런스
- **Knights in the Nightmare** — 고딕 판타지, 파티클 이펙트, 어두운 분위기
- **Yggdra Union** — 섬세한 2D 일러스트, 카드+유닛 혼합
- **Unicorn Overload** — 아기자기한 유닛 표현, 전략 요소
- **Advance Wars** — 맵 기반 스테이지 선택, 직관적 UI
- **Touhou Project** — 탄막 밀도, 화려한 이펙트

### 방향
- **다크 고딕 + 치비**: 세계관은 어둡고 무겁지만 캐릭터·유닛은 SD/치비 비율
- **명암 대비 최우선**: 어두운 배경 + 네온 발광 탄막/캐릭터
- **영혼·파티클**: 캐릭터 주변 빛 입자, 반투명 이펙트 강조
- **고딕 UI 모티브**: 오너먼트, 문장, 스테인드글라스

---

## 컬러 팔레트

### 배경 / 환경
| 역할 | 색상 |
|---|---|
| 배경 베이스 | `#080810` 거의 검정 |
| 성벽/구조물 | `#1A1530` 다크 퍼플그레이 |
| UI 패널 | `#12101E` 반투명 다크 |
| 텍스트 | `#E8DFC0` 크림 화이트 |
| 비활성 | `#3A3555` 뮤트 퍼플 |

### 플레이어 / 아군
| 역할 | 색상 |
|---|---|
| 플레이어 주색 | `#1E6FFF` 브라이트 블루 |
| 플레이어 보조 | `#0A2A6E` 다크 블루 |
| 영혼 글로우 | `#7AB8FF` 페일 블루 |
| UI 포인트 | `#C8A84B` 골드 앰버 |

### 적 / 탄막
| 역할 | 색상 |
|---|---|
| 탄막 A | `#00FF66` 네온 그린 |
| 탄막 B | `#CC44FF` 네온 퍼플 |
| 탄막 C | `#FF2222` 네온 레드 |
| 탄막 D | `#FF66AA` 핫 핑크 |
| 위험 UI | `#FF3030` |

---

## 화면 구성

### MainMenu
```
┌─────────────────────────────────────────┐
│  [사이드바]  │                           │
│  👤 캐릭터  │     성 탈환 맵 (스크롤)    │
│  📖 도감    │                           │
│  ⚙️ 설정    │  ○ ──── ○ ──── ○         │
│             │  노드   노드   노드        │
│             │              ↑           │
│             │         [선택된 노드 팝업] │
└─────────────────────────────────────────┘
```
- 맵: 드래그 스크롤 (2D World Space + Physics2D Raycast)
- 노드: 열림 / 잠김 / 클리어 상태 시각화
- 사이드바: 슬라이드인 (Character / Collection / Settings)
- 노드 팝업: 스테이지명, 클리어 여부, 최고점수, 시작 버튼

### 파티 편성 (StageSelect)
- 6인 중 3인 선택
- 영웅 카드 그리드 (SD 일러스트 + 영혼 글로우)
- 각 영웅 스킬 슬롯 2개 (CW / CCW)

### 씬 전환 흐름
```
Splash → Patch → Intro → DataLoad → MainMenu → InGame
```

---

## 기술 스택

- **엔진**: Unity 6 / C#
- **DI**: VContainer
- **반응형**: R3
- **비동기**: UniTask
- **UI 애니메이션**: DOTweenPro
- **Input**: 순수 C# InputManager (마우스 특화)

---

## 아키텍처 — MVVM + Coordinator

```
EntryPoint (Coordinator)
  └─ UINavigator.LoadAsync<TView, TViewModel, TModel>(
         path, model, canvasType,
         onComplete,  // ViewModel 시퀀스 끝에 수동 호출 (씬 전환)
         onReveal,    // ShowAsync 완료 후 자동 호출
         onHide)      // HideAsync 완료 후 자동 호출
```

### 핵심 규칙
- **LoadAsync** = Initialize → ShowAsync → OnReveal 자동 처리
- **캐시 히트** 시 Initialize 건너뜀, UpdateModel + ShowAsync 실행
- **HideViewAsync()** = `uiNavigator.HideAsync<TView>()` 경유 (View 직접 호출 금지)
- **Model** = 순수 데이터만. 액션 콜백은 `ViewModelBase.OnComplete`
- **SO 접근** = 항상 `DataManager` 경유 (직접 주입 지양)

### EntryPoint 표준 패턴
```csharp
await uiNavigator.LoadAsync<XxxView, XxxViewModel, XxxModel>(
    path:       dataManager.UIConfig.uiXxxView,
    model:      new XxxModel(),
    canvasType: CanvasType.Hud,
    onComplete: () => dataManager.ScenePath.LoadXxxAsync());
```

### ViewModel 시퀀스 패턴
```csharp
async UniTaskVoid XxxSequenceAsync()
{
    // ... 로직 ...
    await HideViewAsync();
    if (OnComplete != null) await OnComplete.Invoke();
}
```

---

## DataManager — 전역 데이터 접근점

```csharp
dataManager.ScenePath    // 씬 전환
dataManager.ResourcePath // 프리팹 생성/해제
dataManager.UIConfig     // UI Prefab Addressable 키
dataManager.SceneConfig  // 씬 이름 목록
dataManager.AppConfig    // CDN URL, 환경, 디버그
dataManager.HeroResource / EnemyResource / StageResource
```

---

## CameraController

```csharp
cameraController.SetMode(CameraMode.Fixed);   // Splash/Intro 등
cameraController.SetMode(CameraMode.MapPan);  // MainMenu 맵 드래그
cameraController.Follow(transform);           // InGame 추적
cameraController.Snap(worldPos);              // 즉시 이동
```

---

## 씬 진행도

| 씬 | 상태 |
|---|---|
| Bootstrap | ✅ |
| Splash | ✅ |
| Patch | ✅ |
| Intro | ✅ |
| DataLoad | ✅ |
| MainMenu | 🔲 맵/노드 UI 미구현, 서브탭 ViewModel 미완성 |
| InGame | 🔲 미착수 |

---

## 다음 할일

1. MainMenu 맵 UI (StageMapView, StageNodeView, StageNodePopupView)
2. MainMenu 서브탭 ViewModel (Character, Collection, Settings)
3. InGame 씬 설계 및 EntryPoint
4. SessionData 설계 (씬 간 파티 구성 전달)
5. DOTween 페이드 애니메이션 (SplashView, IntroView)

---

## 명명 규칙

- 필드: `DataManager dataManager` (타입명 camelCase, 언더바 없음)
- 충돌 시: `this.xxx = xxx`
- SO 클래스: 접미사 `SO` 필수
- EntryPoint / LifetimeScope: namespace 없음
- 커밋: `:이모지:[Type] 메세지` / Claude 작업: `:이모지:[Type][Claude] 메세지`

---

## 주요 파일 경로

```
Assets/01_Scripts/
├── _Core/
│   ├── UI/              ViewBase, UINavigator, ViewModelBase, ViewModelCore
│   ├── Camera/          CameraController
│   ├── Input/           InputManager
│   ├── EntryPoints/     Splash/Patch/Intro/DataLoad EntryPoint
│   └── EntryPointBase, DataManager
├── LifetimeScopes/      AppLifetimeScope
└── [씬명]/UI/           View, ViewModel, Model
```
