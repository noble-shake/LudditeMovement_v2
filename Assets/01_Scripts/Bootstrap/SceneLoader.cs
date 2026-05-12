using UnityEngine.SceneManagement;

using Cysharp.Threading.Tasks;

using RottenNoble.Core;

/// <summary>
/// 씬 전환 서비스
/// BootStrapper → Splash → Patch → Intro → DataLoad → MainMenu → InGame
/// </summary>
public class SceneLoader
{
    readonly SceneConfigSO sceneConfig;

    public SceneLoader(SceneConfigSO sceneConfig)
    {
        this.sceneConfig = sceneConfig;
    }

    public UniTask LoadSplashAsync()   => LoadSingleAsync(sceneConfig.sceneSplash);
    public UniTask LoadPatchAsync()    => LoadSingleAsync(sceneConfig.scenePatch);
    public UniTask LoadIntroAsync()    => LoadSingleAsync(sceneConfig.sceneIntro);
    public UniTask LoadDataLoadAsync() => LoadSingleAsync(sceneConfig.sceneDataLoad);
    public UniTask LoadMainMenuAsync() => LoadSingleAsync(sceneConfig.sceneMainMenu);

    public UniTask LoadInGameAsync(SessionData sessionData)
    {
        SessionData.Current = sessionData;
        return LoadSingleAsync(sceneConfig.sceneInGame);
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
