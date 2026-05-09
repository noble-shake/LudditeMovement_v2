using VContainer;
using VContainer.Unity;

/// <summary>
/// Intro 씬 스코프
/// 게임 타이틀 화면 및 시작 지점 진입
/// Parent: AppLifetimeScope (Inspector에서 설정)
/// </summary>
public class IntroLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterEntryPoint<IntroEntryPoint>();
    }
}
