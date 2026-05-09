using VContainer;
using VContainer.Unity;

/// <summary>
/// DataLoad 씬 스코프
/// 게임 플레이에 필요한 리소스 사전 로드 (Addressables)
/// 완료 후 MainMenu로 전환
/// Parent: AppLifetimeScope
/// </summary>
public class DataLoadLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterEntryPoint<DataLoadEntryPoint>();
    }
}
