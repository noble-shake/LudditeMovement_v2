using System.Threading;
using UnityEngine;

using Cysharp.Threading.Tasks;
using VContainer;
using VContainer.Unity;

using RottenNoble.Core;
using RottenNoble.Core.Resource;
using RottenNoble.Splash.UI;

/// <summary>
/// Splash 씬 EntryPoint — SplashView 프리팹 생성 → SplashViewModel에 위임
/// </summary>
public class SplashEntryPoint : EntryPointBase, IAsyncStartable
{
    [Inject]
    public SplashEntryPoint(ResourceFactory resourceFactory)
    {
        this.resourceFactory = resourceFactory;
    }

    public async UniTask StartAsync(CancellationToken cancellation)
    {
        var viewGO = await resourceFactory.CreateAsync<GameObject>(
            ResourceType.Addressable, UIAddress.SplashView);

        AddCache(ResourceType.Addressable, viewGO);

        var view = viewGO.GetComponent<SplashView>();
        view.Initialize();
        view.InjectPresenter<SplashViewModel>()
            .Initialize(view, new SplashModel());

        await UniTask.WaitUntil(
            () => view.VisibleState == VisibleState.Disappeared,
            cancellationToken: cancellation);
    }
}
