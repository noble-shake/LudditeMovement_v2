using UnityEngine.SceneManagement;

using Cysharp.Threading.Tasks;

using RottenNoble.Core;

/// <summary>
/// 씬 전환 서비스
/// BootStrapper → Splash → Patch → Intro → DataLoad → MainMenu → InGame
/// </summary>
public class SceneLoader
{
    readonly GameConfig _config;

    public SceneLoader(GameConfig config)
    {
        _config = config;
    }

    public UniTask LoadSplashAsync()   => LoadSingleAsync(_config.sceneSplash);
    public UniTask LoadPatchAsync()    => LoadSingleAsync(_config.scenePatch);
    public UniTask LoadIntroAsync()    => LoadSingleAsync(_config.sceneIntro);
    public UniTask LoadDataLoadAsync() => LoadSingleAsync(_config.sceneDataLoad);
    public UniTask LoadMainMenuAsync() => LoadSingleAsync(_config.sceneMainMenu);

    public UniTask LoadInGameAsync(SessionData sessionData)
    {
        SessionData.Current = sessionData;
        return LoadSingleAsync(_config.sceneInGame);
    }

    async UniTask LoadSingleAsync(string sceneName)
    {
        var op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        op.allowSceneActivation = false;

        while (op.progress < 0.9f)
            await UniTask.Yield();

        op.allowSceneActivation = true;
        await UniTask.WaitUntil(() => op.isDone);
    }
}
