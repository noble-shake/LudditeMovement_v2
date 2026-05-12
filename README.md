# SoulHeroes

탄막 슈팅 + SRPG 하이브리드 게임. Steam 출시 목표 솔로 인디 프로젝트.

---

## 개발 환경

| 항목 | 내용 |
|---|---|
| Engine | Unity 6 |
| IDE | VSCode + OmniSharp |
| 언어 | C# |
| 타겟 플랫폼 | PC (Steam), 모바일 가로 모드(Landscape) 추후 지원 |
| 모바일 노치 위치 | 오른쪽 (우측 노치 기준 SafeArea 처리) |

---

## 패키지 / 라이브러리

### Unity Package Manager (UPM)

| 패키지 | 버전 | 용도 |
|---|---|---|
| `jp.hadashikick.vcontainer` | 1.17.0 | DI 컨테이너 |
| `com.cysharp.unitask` | 2.5.10 | 비동기 처리 |
| `com.cysharp.r3` | git (latest) | Reactive Extensions |
| `com.unity.addressables` | 2.9.1 | 에셋 번들 / 원격 로드 |
| `com.unity.ugui` | 2.0.0 | UI Toolkit |
| `com.unity.inputsystem` | 1.19.0 | 입력 처리 |
| `com.unity.render-pipelines.universal` | 17.4.0 | URP |
| `com.unity.ide.visualstudio` | 2.0.23 | VSCode용 .sln 생성 (필수) |
| `com.unity.device-simulator.devices` | 1.0.1 | 디바이스 시뮬레이터 |
| `com.github-glitchenzo.nugetforunity` | git (latest) | NuGet 패키지 관리 |

> **`com.unity.ide.visualstudio` 필수 이유**
> Unity 6는 기본으로 `.slnx` 포맷을 생성하는데 OmniSharp가 이를 인식하지 못함.
> 이 패키지가 있어야 OmniSharp 호환 `.sln` + `.csproj`가 생성됨.

### Asset Store / 수동 설치 (git 미포함)

| 패키지 | 경로 | 용도 |
|---|---|---|
| **DOTween Pro** | `Assets/Plugins/Demigiant/` | 트윈 애니메이션 |

> **DOTween Pro 설치 방법**
> 1. [Asset Store](https://assetstore.unity.com/packages/tools/animation/dotween-hotween-v2-27676) 에서 구매 후 Unity Package Manager로 import
> 2. `Assets/Plugins/Demigiant/` 경로에 설치됨
> 3. 설치 후 `Tools → DOTween Utility Panel → Setup DOTween…` 실행 필수
>
> 해당 폴더는 `.gitignore`에 등록되어 있으므로 팀원 각자 직접 설치해야 합니다.

### NuGet (via NuGetForUnity)

| 패키지 | 버전 | 용도 |
|---|---|---|
| `Newtonsoft.Json` | 13.0.4 | JSON 직렬화 |

---

## VSCode 설정 (`.vscode/settings.json`)

```json
"dotnet.defaultSolution": "SoulHeroes.sln",
"omnisharp.useModernNet": false,
"omnisharp.enableRoslynAnalyzers": false
```

- `useModernNet: false` — Unity .csproj 호환을 위해 필수
- `defaultSolution` — `.slnx` 아닌 `.sln` 명시 지정

---

## 아키텍처

### 패턴

**MVVM + EntryPoint** (VContainer 기반)

```
EntryPoint
  └─ UINavigator.LoadAsync<TView, TViewModel, TModel>(path, model, onReveal, onHide)
       ├─ Initialize(view, model)    ← ViewModel 초기화
       ├─ view.ShowAsync()           ← 자동 표시 (페이드인 등)
       └─ onReveal?.Invoke()         ← 표시 완료 콜백

씬 전환 패턴 (Model-Action)
  └─ Model.OnComplete = () => dataManager.ScenePath.LoadXxxAsync()
       └─ ViewModel이 시퀀스 완료 후 Model.OnComplete 호출
```

### 씬 흐름

```
BootStrapper → Splash → Patch → Intro → DataLoad → MainMenu → InGame
```

각 씬마다 `LifetimeScope` + `EntryPoint` 쌍으로 구성.  
모든 씬의 Parent Scope → `AppLifetimeScope` (DontDestroyOnLoad).

### 핵심 클래스 계층

```
EntryPointBase          ← 씬 EntryPoint 공통 기반 (리소스 캐시 관리)
  └─ SplashEntryPoint, PatchEntryPoint, ...

ViewModelCore           ← 모든 ViewModel 공통 기반 (서비스 주입)
  └─ ViewModelBase<TView, TModel>
       └─ SplashViewModel, MainMenuViewModel, ...

ViewBase                ← 모든 View 공통 기반 (ShowAsync / HideAsync 계약)
  └─ SplashView, MainMenuView, ...
```

### AppLifetimeScope 등록 구조

```
GameConfig          (ScriptableObject, Inspector 연결)
UIManager           (MonoBehaviour, Canvas 레이어 관리)
UINavigator         (View 로드 → Canvas 배치 → 초기화)
ResourceManager     (IResourceManager 구현, Addressables)
ResourceFactory     (프리팹 생성/해제)
SceneLoader         (씬 전환)
DataManager         (.Scene → SceneLoader, .Resource → ResourceFactory)
SaveDataService     (세이브/로드)
AudioService        (BGM/SFX)
PatchService        (Addressables 카탈로그 업데이트)
DataLoadService     (게임 데이터 사전 로드)
StageSelectService  (스테이지 선택 비즈니스 로직)
```

---

## Canvas 레이어

| Sort Order | 레이어 | 용도 |
|---|---|---|
| 300 | Overlay | 씬 전환 페이드, 로딩 화면 |
| 200 | Popup | 다이얼로그, 설정창, 모달 UI |
| 100 | Hud | 인게임 HUD, 메인메뉴 기본 UI |

- 각 Canvas 하위에 `SafeAreaRectTransform` 컴포넌트를 붙인 `SafeAreaContainer` 배치
- 배경/장식 요소 → Canvas 직하위 (SafeArea 밖까지 full screen)
- 버튼/텍스트 등 인터랙티브 요소 → SafeAreaContainer 안

---

## ScriptableObject 설정 파일

### `GameConfig` (`Assets/` 우클릭 → Create → RottenNoble → Game Config)

```
[ Scene Names ]         [ UI Prefab Keys (Addressable) ]
sceneBootstrap          uiSplashView
sceneSplash             uiPatchView
scenePatch              uiIntroView
sceneIntro              uiDataLoadView
sceneDataLoad           uiMainMenuView
sceneMainMenu
sceneInGame
```

`AppLifetimeScope` Inspector의 `Game Config` 필드에 연결 필수.

---

## 명명 규칙

### Namespace

```csharp
namespace RottenNoble.Core          // Core 시스템
namespace RottenNoble.Core.UI       // UI 기반 클래스
namespace RottenNoble.Core.Resource // 리소스 관리
namespace RottenNoble.MainMenu.xxx  // 씬별 기능
```

- `LifetimeScope`, `EntryPoint` 파일은 **namespace 없음** (VContainer 탐색 용이)
- 브랜드명은 `RottenNoble` (SoulHeroes X)

### using 순서

```csharp
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
                            // ← 빈 줄
using Cysharp.Threading.Tasks;
using R3;
using VContainer;
using VContainer.Unity;
                            // ← 빈 줄
using RottenNoble.Core;
using RottenNoble.Core.UI;
```

### 필드 명명

| 종류 | 규칙 | 예시 |
|---|---|---|
| 매니저/서비스 필드 | 타입명 camelCase, 언더바 없음 | `DataManager dataManager` |
| 일반 private 필드 | camelCase, 언더바 없음 | `bool disposed` |
| 이름 충돌 시 | `this.` 사용 | `this.dataManager = dataManager` |
| **예외** (인터페이스/리졸버) | 언더바 허용 | `IObjectResolver _objectResolver` |

```csharp
// ✅ 올바른 예
readonly DataManager    dataManager;
readonly AudioService   audioService;
readonly SaveDataService saveDataService;
readonly GameConfig     gameConfig;
bool                    disposed;

// ❌ 잘못된 예
readonly DataManager _data;        // 단축 별칭 + 언더바
readonly AudioService _audio;      // 단축 별칭 + 언더바
DataManager data;                  // 단축 별칭 (타입명과 다름)
```

### 클래스/파일 명명

| 종류 | 규칙 | 예시 |
|---|---|---|
| View | `{기능}View` | `SplashView`, `MainMenuView` |
| ViewModel | `{기능}ViewModel` | `SplashViewModel` |
| Model | `{기능}Model` | `SplashModel` |
| EntryPoint | `{씬}EntryPoint` | `SplashEntryPoint` |
| LifetimeScope | `{씬}LifetimeScope` | `SplashLifetimeScope` |
| Service | `{기능}Service` | `StageSelectService` |

---

## 자주 쓰는 패턴

### EntryPoint에서 View 띄우기

```csharp
// LoadAsync 한 번으로 Initialize → ShowAsync → OnReveal 까지 처리됨
var model = new SplashModel
{
    OnComplete = () => dataManager.ScenePath.LoadPatchAsync()
};

await uiNavigator.LoadAsync<SplashView, SplashViewModel, SplashModel>(
    path:       uiConfig.uiSplashView,
    model:      model,
    canvasType: CanvasType.Hud);
// UINavigator가 View를 자동 추적 → EntryPoint.Dispose 시 DestroyAll() 자동 호출
```

### ViewModel에서 씬 전환 (Model-Action 패턴)

```csharp
// ViewModel 시퀀스 완료 후 EntryPoint가 주입한 액션 실행
await View.HideAsync();
if (Model.OnComplete != null)
    await Model.OnComplete.Invoke();
```

### 숨겨진 View 재표시 (re-show)

```csharp
await uiNavigator.ShowAsync<SomeView>();  // HideAsync 후 다시 표시할 때만 사용
```

### `Object` 모호성 해결 (System + UnityEngine 동시 using 시)

```csharp
using Object = UnityEngine.Object;
```

---

## 커스텀 에디터 툴

- **Scene Navigator** — `RottenNoble → Scene Navigator`
  - Build Settings 씬 목록 표시
  - BootStrapper 상단 고정
  - 미등록 씬 경고 + "+ Build" 버튼
