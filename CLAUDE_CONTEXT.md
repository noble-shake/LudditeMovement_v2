# SoulHeroes — Claude Context

> 이 파일을 채팅 첫 메세지에 붙여넣으면 프로젝트 컨텍스트가 복원됩니다.

---

## 프로젝트

- **게임**: 탄막 슈팅 + 실시간 유닛 관리 하이브리드 (Steam 출시 목표, 솔로 인디)
- **레포**: `github.com/noble-shake/LudditeMovement_v2` / 브랜치 `feature/renewal`
- **경로**: `E:\workspace\SoulHeroes`
- **엔진**: Unity 6 / C#

---

## 기술 스택

- DI: VContainer / 반응형: R3 / 비동기: UniTask
- UI 애니메이션: DOTweenPro (`Assets/Plugins/Demigiant/`, git 미포함)
- Input: 순수 C# InputManager (마우스 특화)

---

## 아키텍처 — MVVM + Coordinator

```
EntryPoint
  └─ uiNavigator.LoadAsync<TView, TViewModel, TModel>(
         path, model, canvasType,
         onComplete,   // ViewModel.OnComplete에 주입 → ViewModel이 시퀀스 끝에 호출
         onReveal,     // ShowAsync 완료 후 Navigator 자동 호출
         onHide)       // HideAsync 완료 후 Navigator 자동 호출
```

### 핵심 규칙
- **LoadAsync** = Initialize → ShowAsync → OnReveal 자동 처리
- **캐시 히트** 시 Initialize 건너뜀, UpdateModel + ShowAsync만 실행
- **HideViewAsync()** = `uiNavigator.HideAsync<TView>()` 경유 (View 직접 호출 금지)
- **Model** = 순수 데이터만. 액션 콜백은 `ViewModelBase.OnComplete`
- **EntryPoint.Dispose()** 시 `DestroyAll()` 자동 호출 (수동 AddUICache 없음)

### EntryPoint 표준 패턴
```csharp
await uiNavigator.LoadAsync<XxxView, XxxViewModel, XxxModel>(
    path:       uiConfig.uiXxxView,
    model:      new XxxModel(),
    canvasType: CanvasType.Hud,
    onComplete: () => dataManager.ScenePath.LoadXxxAsync());
```

### ViewModel 시퀀스 패턴
```csharp
async UniTaskVoid XxxSequenceAsync()
{
    // ... 로직 ...
    await HideViewAsync();          // Navigator 경유 Hide
    if (OnComplete != null)
        await OnComplete.Invoke();  // 씬 전환 등
}
```

---

## 씬 진행도

| 씬 | 상태 |
|---|---|
| Bootstrap | ✅ |
| Splash | ✅ 2s 대기 → Hide → LoadPatch |
| Patch | ✅ CheckAndUpdate → Hide → LoadIntro |
| Intro | ✅ 좌클릭 → Hide → LoadDataLoad |
| DataLoad | ✅ LoadAll → Hide → LoadMainMenu |
| MainMenu | 🔲 탭 전환만, 서브탭 ViewModel 미구현 |
| InGame | 🔲 미착수 |

---

## 다음 할일

1. MainMenu 서브탭 ViewModel 구현 (StageSelect, Character, Collection, Settings)
2. InGame 씬 설계
3. SessionData 설계 (씬 간 파티 구성 전달)
4. DOTween 페이드 애니메이션 적용

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
│   ├── UI/ViewBase.cs, UINavigator.cs, ViewModelBase.cs, ViewModelCore.cs
│   ├── EntryPointBase.cs
│   ├── EntryPoints/  (Splash/Patch/Intro/DataLoad EntryPoint)
│   └── Input/InputManager.cs
├── LifetimeScopes/AppLifetimeScope.cs
└── [씬명]/UI/  (View, ViewModel, Model)
```
