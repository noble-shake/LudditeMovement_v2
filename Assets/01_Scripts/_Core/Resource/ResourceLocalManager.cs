using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

using Cysharp.Threading.Tasks;

namespace RottenNoble.Core.Resource
{
    /// <summary>
    /// Resources 폴더 기반 로컬 에셋 관리자
    /// </summary>
    public class ResourceLocalManager : IResourceManager
    {
        readonly Dictionary<string, UnityEngine.Object> _assetCache    = new();
        readonly Dictionary<int,    UnityEngine.Object> _instanceCache = new();

        public async UniTask<T> LoadAsync<T>(string path, Action<string, float> onProgress = null)
            where T : UnityEngine.Object
        {
            if (_assetCache.TryGetValue(path, out var cached))
            {
                onProgress?.Invoke(path, 1f);
                return cached as T;
            }

            var req = Resources.LoadAsync<T>(path);
            while (!req.isDone)
            {
                onProgress?.Invoke(path, req.progress);
                await UniTask.Yield();
            }

            _assetCache[path] = req.asset;
            return req.asset as T;
        }

        public async UniTask<List<T>> LoadsAsync<T>(List<string> paths, Action<string, float> onProgress = null)
            where T : UnityEngine.Object
        {
            var results = new List<T>(paths.Count);
            foreach (var p in paths) results.Add(await LoadAsync<T>(p, onProgress));
            return results;
        }

        public async UniTask<List<T>> LoadByLabelAsync<T>(string label, Action<string, float> onProgress = null)
            where T : UnityEngine.Object
        {
            var assets = Resources.LoadAll<T>(label);
            onProgress?.Invoke(label, 1f);
            await UniTask.CompletedTask;
            return new List<T>(assets);
        }

        public async UniTask<T> CreateInstanceAsync<T>(string path, Transform parent = null)
            where T : UnityEngine.Object
        {
            var asset    = await LoadAsync<T>(path);
            var instance = UnityEngine.Object.Instantiate(asset, parent);
            instance.name = instance.name.Replace("(Clone)", "").Trim();
            _instanceCache[instance.GetInstanceID()] = instance;
            return instance;
        }

        public async UniTask<List<T>> CreateInstancesAsync<T>(List<string> paths, Transform parent = null)
            where T : UnityEngine.Object
        {
            var results = new List<T>(paths.Count);
            foreach (var p in paths) results.Add(await CreateInstanceAsync<T>(p, parent));
            return results;
        }

        public void DeleteInstance(UnityEngine.Object instanceObject)
        {
            if (instanceObject == null) return;
            int id = instanceObject.GetInstanceID();
            if (_instanceCache.TryGetValue(id, out var obj))
            {
                UnityEngine.Object.Destroy(obj);
                _instanceCache.Remove(id);
            }
        }

        public void Dispose()
        {
            foreach (var obj in _instanceCache.Values)
                if (obj != null) UnityEngine.Object.Destroy(obj);
            _instanceCache.Clear();
            _assetCache.Clear();
            Resources.UnloadUnusedAssets();
        }
    }
}
