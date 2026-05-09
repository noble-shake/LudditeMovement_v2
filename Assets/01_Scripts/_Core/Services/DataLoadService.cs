using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;

using Cysharp.Threading.Tasks;
using R3;

namespace RottenNoble.Core.Services
{
    /// <summary>
    /// 게임 플레이에 필요한 리소스 Addressables 사전 로드
    /// </summary>
    public class DataLoadService
    {
        public ReactiveProperty<float>  Progress   { get; } = new(0f);
        public ReactiveProperty<string> StatusText { get; } = new(string.Empty);

        static class Labels
        {
            public const string HeroData    = "HeroData";
            public const string MonsterData = "MonsterData";
            public const string StageData   = "StageData";
            public const string UI          = "UI";
            public const string Audio       = "Audio";
        }

        readonly string[] loadOrder =
        {
            Labels.HeroData,
            Labels.MonsterData,
            Labels.StageData,
            Labels.UI,
            Labels.Audio,
        };

        public async UniTask LoadAllAsync(CancellationToken cancellation = default)
        {
            int total = loadOrder.Length;

            for (int i = 0; i < total; i++)
            {
                var label = loadOrder[i];
                StatusText.Value = $"{label} 로드 중...";

                await Addressables.LoadAssetsAsync<Object>(
                    label, _ => { }
                ).ToUniTask(cancellationToken: cancellation);

                Progress.Value = (float)(i + 1) / total;
            }

            StatusText.Value = "로드 완료";
        }
    }
}
