using Cysharp.Threading.Tasks;
using VContainer;
using VContainer.Unity;

/// <summary>
/// InGame 씬 진입점 - 스테이지 초기화 및 시작
/// </summary>
public class InGameEntryPoint : IAsyncStartable
{
    // readonly SessionData      _session;
    // readonly StageManager     _stageManager;
    // readonly HeroManager      _heroManager;
    // readonly SoulGaugeSystem  _soulGauge;

    // [Inject]
    // public InGameEntryPoint(
    //     SessionData     session,
    //     StageManager    stageManager,
    //     HeroManager     heroManager,
    //     SoulGaugeSystem soulGauge)
    // {
    //     _session      = session;
    //     _stageManager = stageManager;
    //     _heroManager  = heroManager;
    //     _soulGauge    = soulGauge;
    // }

    

    public async UniTask StartAsync(System.Threading.CancellationToken cancellation)
    {
        // 1. 스테이지 데이터 로드 및 맵 생성
        // await _stageManager.LoadStageAsync(_session.StageId, cancellation);

        // // 2. 파티 초기화 (선택된 영웅 4명)
        // _heroManager.InitializeParty(_session.SelectedHeroes, _session.SkillLoadouts);

        // // 3. 소울 게이지 초기화
        // _soulGauge.Initialize();

        // // 4. 스테이지 시작
        // _stageManager.StartStage();
    }
}
