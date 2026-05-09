using VContainer;
using VContainer.Unity;

/// <summary>
/// MainMenu 씬 스코프
/// Parent: AppLifetimeScope (Inspector에서 설정)
/// </summary>
public class MainMenuLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterEntryPoint<MainMenuEntryPoint>();
    }
}
