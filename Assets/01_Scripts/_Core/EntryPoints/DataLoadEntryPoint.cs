using System.Threading;
using UnityEngine;

using Cysharp.Threading.Tasks;
using VContainer;
using VContainer.Unity;

using RottenNoble.Core;
using RottenNoble.Core.Resource;
using RottenNoble.DataLoad.UI;

/// <summary>
/// DataLoad 씬 EntryPoint — DataLoadView 프리팹 생성 → DataLoadViewModel에 위임
/// </summary>
public class DataLoadEntryPoint : EntryPointBase, IAsyncStartable
{
    [Inject]
    public DataLoadEntryPoint(ResourceFactory resourceFactory)
    {
        this.resourceFactory = resourceFactory;
    }

    public async UniTask StartAsync(CancellationToken cancellation)
    {
        var viewGO = await resourceFactory.CreateAsync<GameObject>(
            ResourceType.Addressable, UIAddress.DataLoadView);

        AddCache(ResourceType.Addressable, viewGO);

        var view = viewGO.GetComponent<DataLoadView>();
        view.Initialize();
        view.InjectPresenter<DataLoadViewModel>()
            .Initialize(view, new DataLoadModel());

        await UniTask.WaitUntil(
            () => view.VisibleState == VisibleState.Disappeared,
            cancellationToken: cancellation);
    }
}
