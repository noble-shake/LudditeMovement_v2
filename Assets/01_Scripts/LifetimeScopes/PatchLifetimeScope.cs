using VContainer;
using VContainer.Unity;

/// <summary>
/// Patch 씬 스코프
/// Addressables 카탈로그 업데이트, 필수 에셋 다운로드 체크
/// Parent: AppLifetimeScope
/// </summary>
public class PatchLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterEntryPoint<PatchEntryPoint>();
    }
}
