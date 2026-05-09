using System.Threading;
using UnityEngine;

using Cysharp.Threading.Tasks;
using VContainer;
using VContainer.Unity;

using RottenNoble.Core;
using RottenNoble.Core.Resource;
using RottenNoble.Intro.UI;

/// <summary>
/// Intro 씬 EntryPoint — IntroView 프리팹 생성 → IntroViewModel에 위임
/// </summary>
public class IntroEntryPoint : EntryPointBase, IAsyncStartable
{
    [Inject]
    public IntroEntryPoint(ResourceFactory resourceFactory)
    {
        this.resourceFactory = resourceFactory;
    }

    public async UniTask StartAsync(CancellationToken cancellation)
    {
        var viewGO = await resourceFactory.CreateAsync<GameObject>(
            ResourceType.Addressable, UIAddress.IntroView);

        AddCache(ResourceType.Addressable, viewGO);

        var view = viewGO.GetComponent<IntroView>();
        view.Initialize();
        view.InjectPresenter<IntroViewModel>()
            .Initialize(view, new IntroModel());

        await UniTask.WaitUntil(
            () => view.VisibleState == VisibleState.Disappeared,
            cancellationToken: cancellation);
    }
}
