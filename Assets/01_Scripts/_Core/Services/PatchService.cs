using System.Collections.Generic;
using System.Threading;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

using Cysharp.Threading.Tasks;
using R3;

namespace RottenNoble.Core.Services
{
    /// <summary>
    /// Addressables 카탈로그 업데이트 및 필수 에셋 다운로드 관리
    /// </summary>
    public class PatchService
    {
        public ReactiveProperty<float>  Progress   { get; } = new(0f);
        public ReactiveProperty<string> StatusText { get; } = new(string.Empty);
        public bool NeedsPatch { get; private set; }

        public async UniTask CheckAndUpdateAsync(CancellationToken cancellation = default)
        {
            StatusText.Value = "업데이트 확인 중...";

            var checkHandle = Addressables.CheckForCatalogUpdates(autoReleaseHandle: false);
            await checkHandle.ToUniTask(cancellationToken: cancellation);

            var catalogsToUpdate = checkHandle.Result;
            Addressables.Release(checkHandle);

            if (catalogsToUpdate == null || catalogsToUpdate.Count == 0)
            {
                NeedsPatch       = false;
                Progress.Value   = 1f;
                StatusText.Value = "최신 버전입니다.";
                return;
            }

            NeedsPatch = true;

            StatusText.Value = "카탈로그 업데이트 중...";
            var updateHandle = Addressables.UpdateCatalogs(catalogsToUpdate, autoReleaseHandle: false);
            await updateHandle.ToUniTask(cancellationToken: cancellation);
            Addressables.Release(updateHandle);

            await DownloadDependenciesAsync(cancellation);
        }

        async UniTask DownloadDependenciesAsync(CancellationToken cancellation)
        {
            const string downloadLabel = "Preload";

            var sizeHandle = Addressables.GetDownloadSizeAsync(downloadLabel);
            await sizeHandle.ToUniTask(cancellationToken: cancellation);

            long downloadSize = sizeHandle.Result;
            Addressables.Release(sizeHandle);

            if (downloadSize <= 0)
            {
                Progress.Value   = 1f;
                StatusText.Value = "다운로드 완료";
                return;
            }

            StatusText.Value = $"다운로드 중... ({downloadSize / 1024 / 1024}MB)";

            var downloadHandle = Addressables.DownloadDependenciesAsync(downloadLabel, autoReleaseHandle: false);
            while (!downloadHandle.IsDone)
            {
                Progress.Value = downloadHandle.GetDownloadStatus().Percent;
                await UniTask.Yield(cancellation);
            }

            Addressables.Release(downloadHandle);
            Progress.Value   = 1f;
            StatusText.Value = "준비 완료";
        }
    }
}
