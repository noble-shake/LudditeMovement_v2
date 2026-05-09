using System.Threading;
using UnityEngine;

using Cysharp.Threading.Tasks;
using VContainer;
using VContainer.Unity;

using RottenNoble.Core;
using RottenNoble.Core.Resource;
using RottenNoble.MainMenu.UI;

/// <summary>
/// MainMenu 씬 EntryPoint — MainMenuView 프리팹 생성 → MainMenuViewModel에 위임
/// </summary>
public class MainMenuEntryPoint : EntryPointBase, IAsyncStartable
{
    [Inject]
    public MainMenuEntryPoint(ResourceFactory resourceFactory)
    {
        this.resourceFactory = resourceFactory;
    }

    public async UniTask StartAsync(CancellationToken cancellation)
    {
        var viewGO = await resourceFactory.CreateAsync<GameObject>(
            ResourceType.Addressable, UIAddress.MainMenuView);

        AddCache(ResourceType.Addressable, viewGO);

        var view = viewGO.GetComponent<MainMenuView>();
        view.Initialize();
        view.InjectPresenter<MainMenuViewModel>()
            .Initialize(view, new MainMenuModel());

        await UniTask.WaitWhile(
            () => view != null && view.VisibleState != VisibleState.Disappeared,
            cancellationToken: cancellation);
    }
}
