using VContainer;
using VContainer.Unity;

/// <summary>
/// Splash 씬 스코프
/// 로고 애니메이션 표시 후 Patch 씬으로 전환
/// Parent: AppLifetimeScope (Inspector에서 설정)
/// </summary>
public class SplashLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterEntryPoint<SplashEntryPoint>();
    }
}
