using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

using Cysharp.Threading.Tasks;
using VContainer.Unity;

namespace RottenNoble.Core.Resource
{
    /// <summary>
    /// ResourceType 별 IResourceManager 구현체를 묶은 Facade
    /// </summary>
    public class ResourceManager : IInitializable, IDisposable
    {
        readonly Dictionary<ResourceType, IResourceManager> _providers = new();

        public void Initialize()
        {
            _providers[ResourceType.Resource]    = new ResourceLocalManager();
            _providers[ResourceType.Addressable] = new ResourceAddressableManager();
        }

        public UniTask<T> LoadAsync<T>(ResourceType type, string path,
            Action<string, float> onProgress = null) where T : Object
            => _providers[type].LoadAsync<T>(path, onProgress);

        public UniTask<List<T>> LoadsAsync<T>(ResourceType type, List<string> paths,
            Action<string, float> onProgress = null) where T : Object
            => _providers[type].LoadsAsync<T>(paths, onProgress);

        public UniTask<List<T>> LoadByLabelAsync<T>(ResourceType type, string label,
            Action<string, float> onProgress = null) where T : Object
            => _providers[type].LoadByLabelAsync<T>(label, onProgress);

        public UniTask<T> CreateInstanceAsync<T>(ResourceType type, string path, Transform parent = null)
            where T : Object
            => _providers[type].CreateInstanceAsync<T>(path, parent);

        public UniTask<List<T>> CreateInstancesAsync<T>(ResourceType type, List<string> paths, Transform parent = null)
            where T : Object
            => _providers[type].CreateInstancesAsync<T>(paths, parent);

        public void DeleteInstance(ResourceType type, Object instanceObject)
            => _providers[type].DeleteInstance(instanceObject);

        public void Dispose()
        {
            foreach (var p in _providers.Values) p.Dispose();
        }
    }
}
