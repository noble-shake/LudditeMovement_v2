using System.Threading;
using UnityEngine;

using Cysharp.Threading.Tasks;
using VContainer;
using VContainer.Unity;

using RottenNoble.Core;
using RottenNoble.Core.Resource;
using RottenNoble.Patch.UI;

/// <summary>
/// Patch 씬 EntryPoint — PatchView 프리팹 생성 → PatchViewModel에 위임
/// </summary>
public class PatchEntryPoint : EntryPointBase, IAsyncStartable
{
    [Inject]
    public PatchEntryPoint(ResourceFactory resourceFactory)
    {
        this.resourceFactory = resourceFactory;
    }

    public async UniTask StartAsync(CancellationToken cancellation)
    {
        var viewGO = await resourceFactory.CreateAsync<GameObject>(
            ResourceType.Addressable, UIAddress.PatchView);

        AddCache(ResourceType.Addressable, viewGO);

        var view = viewGO.GetComponent<PatchView>();
        view.Initialize();
        view.InjectPresenter<PatchViewModel>()
            .Initialize(view, new PatchModel());

        await UniTask.WaitUntil(
            () => view.VisibleState == VisibleState.Disappeared,
            cancellationToken: cancellation);
    }
}
