using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

using Cysharp.Threading.Tasks;

namespace RottenNoble.Core.Resource
{
    /// <summary>
    /// Addressables 기반 원격/로컬 에셋 관리자
    /// </summary>
    public class ResourceAddressableManager : IResourceManager
    {
        readonly Dictionary<string, Object>                            _assetCache = new();
        readonly Dictionary<string, AsyncOperationHandle>              _handles    = new();
        readonly Dictionary<int, (Object Obj, Action<Object> Release)> _instances  = new();

        public async UniTask<T> LoadAsync<T>(string path, Action<string, float> onProgress = null)
            where T : Object
        {
            if (_assetCache.TryGetValue(path, out var cached))
                return cached as T;

            var handle = Addressables.LoadAssetAsync<T>(path);
            while (!handle.IsDone)
            {
                onProgress?.Invoke(path, handle.PercentComplete);
                await UniTask.Yield();
            }

            if (handle.Status != AsyncOperationStatus.Succeeded)
                throw new Exception($"[ResourceAddressable] Load failed: {path}");

            _assetCache[path] = handle.Result;
            _handles[path]    = handle;
            return handle.Result;
        }

        public async UniTask<List<T>> LoadsAsync<T>(List<string> paths, Action<string, float> onProgress = null)
            where T : Object
        {
            var results = new List<T>(paths.Count);
            foreach (var p in paths) results.Add(await LoadAsync<T>(p, onProgress));
            return results;
        }

        public async UniTask<List<T>> LoadByLabelAsync<T>(string label, Action<string, float> onProgress = null)
            where T : Object
        {
            var handle = Addressables.LoadAssetsAsync<T>(label, null);
            if (!_handles.ContainsKey(label))
                _handles[label] = handle;

            while (!handle.IsDone)
            {
                onProgress?.Invoke(label, handle.PercentComplete);
                await UniTask.Yield();
            }

            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                Addressables.Release(handle);
                _handles.Remove(label);
                throw new Exception($"[ResourceAddressable] Label load failed: {label}");
            }

            return new List<T>(handle.Result);
        }

        public async UniTask<T> CreateInstanceAsync<T>(string path, Transform parent = null)
            where T : Object
        {
            if (typeof(T) == typeof(GameObject) || typeof(T).IsSubclassOf(typeof(Component)))
            {
                var h = Addressables.InstantiateAsync(path, parent);
                while (!h.IsDone) await UniTask.Yield();

                if (h.Status != AsyncOperationStatus.Succeeded)
                    throw new Exception($"[ResourceAddressable] Instantiate failed: {path}");

                var go  = h.Result;
                go.name = go.name.Replace("(Clone)", "").Trim();

                if (typeof(T) == typeof(GameObject))
                {
                    RegisterInstance(go, obj => Addressables.ReleaseInstance((GameObject)obj));
                    return go as T;
                }
                else
                {
                    var comp = go.GetComponent<T>();
                    RegisterInstance(go, obj => Addressables.ReleaseInstance((GameObject)obj));
                    return comp;
                }
            }

            var asset = await LoadAsync<T>(path);
            var inst  = Object.Instantiate(asset, parent);
            inst.name = inst.name.Replace("(Clone)", "").Trim();
            RegisterInstance(inst, obj => Object.Destroy(obj));
            return inst;
        }

        public async UniTask<List<T>> CreateInstancesAsync<T>(List<string> paths, Transform parent = null)
            where T : Object
        {
            var results = new List<T>(paths.Count);
            foreach (var p in paths) results.Add(await CreateInstanceAsync<T>(p, parent));
            return results;
        }

        public void DeleteInstance(Object instanceObject)
        {
            if (instanceObject == null) return;
            var id = instanceObject.GetEntityId();
            if (!_instances.TryGetValue(id, out var entry)) return;

            entry.Release?.Invoke(entry.Obj);
            _instances.Remove(id);
        }

        public void Dispose()
        {
            foreach (var entry in _instances.Values)
                entry.Release?.Invoke(entry.Obj);
            _instances.Clear();

            foreach (var h in _handles.Values)
                if (h.IsValid()) Addressables.Release(h);
            _handles.Clear();
            _assetCache.Clear();
        }

        void RegisterInstance(Object obj, Action<Object> release)
        {
            var id = obj.GetEntityId();
            if (!_instances.ContainsKey(id))
                _instances[id] = (obj, release);
        }
    }
}
