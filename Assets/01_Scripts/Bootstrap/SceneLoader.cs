using UnityEngine.SceneManagement;

using Cysharp.Threading.Tasks;

namespace RottenNoble.Core
{
    /// <summary>
    /// 씬 전환 서비스
    /// BootStrapper → Splash → Patch → Intro → DataLoad → MainMenu → InGame
    /// </summary>
    public class SceneLoader
    {
        public static class SceneName
        {
            public const string BootStrapper = "BootStrapper";
            public const string Splash       = "Splash";
            public const string Patch        = "Patch";
            public const string Intro        = "Intro";
            public const string DataLoad     = "DataLoad";
            public const string MainMenu     = "MainMenu";
            public const string InGame       = "InGame";
        }

        public UniTask LoadSplashAsync()   => LoadSingleAsync(SceneName.Splash);
        public UniTask LoadPatchAsync()    => LoadSingleAsync(SceneName.Patch);
        public UniTask LoadIntroAsync()    => LoadSingleAsync(SceneName.Intro);
        public UniTask LoadDataLoadAsync() => LoadSingleAsync(SceneName.DataLoad);
        public UniTask LoadMainMenuAsync() => LoadSingleAsync(SceneName.MainMenu);

        public UniTask LoadInGameAsync(SessionData sessionData)
        {
            SessionData.Current = sessionData;
            return LoadSingleAsync(SceneName.InGame);
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
}
