using VContainer;
using VContainer.Unity;

using RottenNoble.Core;

/// <summary>
/// InGame 씬 스코프
/// Parent: AppLifetimeScope (Inspector에서 설정)
/// </summary>
public class InGameLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        // 세션 데이터 (MainMenu에서 전달받은 파티/스킬/스테이지 정보)
        builder.RegisterInstance(SessionData.Current);

        // TODO: 게임 시스템 순차 등록 예정
        // builder.Register<SoulGaugeSystem>(Lifetime.Scoped);
        // builder.Register<HeroManager>(Lifetime.Scoped);
        // builder.Register<MonsterManager>(Lifetime.Scoped);
        // builder.Register<StageManager>(Lifetime.Scoped);
        // builder.RegisterEntryPoint<InGameEntryPoint>();
    }
}
