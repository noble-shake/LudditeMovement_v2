using VContainer;
using VContainer.Unity;

/// <summary>
/// Bootstrap 씬 스코프
/// 앱 시작 시 한 번만 실행 - 초기화 후 MainMenu로 전환
/// Parent: AppLifetimeScope
/// </summary>
public class BootstrapLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterEntryPoint<BootstrapEntryPoint>();
    }
}
