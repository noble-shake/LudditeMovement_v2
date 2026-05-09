using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

using Cysharp.Threading.Tasks;
using VContainer;
using VContainer.Unity;


namespace RottenNoble.Core.Resource
{
    /// <summary>
    /// 리소스 로드 + VContainer 인젝션을 한번에 처리하는 팩토리
    /// </summary>
    public class ResourceFactory
    {
        readonly IObjectResolver _resolver;
        readonly ResourceManager _manager;

        [Inject]
        public ResourceFactory(IObjectResolver resolver, ResourceManager manager)
        {
            _resolver = resolver;
            _manager  = manager;
        }

        public async UniTask<T> LoadAsync<T>(ResourceType type, string path,
            Action<string, float> onProgress = null) where T : Object
        {
            var asset = await _manager.LoadAsync<T>(type, path, onProgress);
            InjectInternal(asset);
            return asset;
        }

        public async UniTask<List<T>> LoadByLabelAsync<T>(ResourceType type, string label,
            Action<string, float> onProgress = null) where T : Object
        {
            var assets = await _manager.LoadByLabelAsync<T>(type, label, onProgress);
            foreach (var a in assets) InjectInternal(a);
            return assets;
        }

        public async UniTask<T> CreateAsync<T>(ResourceType type, string path, Transform parent = null)
            where T : Object
        {
            var instance = await _manager.CreateInstanceAsync<T>(type, path, parent);
            InjectInternal(instance);
            return instance;
        }

        public async UniTask<List<T>> CreateManyAsync<T>(ResourceType type, List<string> paths, Transform parent = null)
            where T : Object
        {
            var instances = await _manager.CreateInstancesAsync<T>(type, paths, parent);
            foreach (var inst in instances) InjectInternal(inst);
            return instances;
        }

        public void DeleteInstance(ResourceType type, Object instanceObject)
            => _manager.DeleteInstance(type, instanceObject);

        void InjectInternal(Object obj)
        {
            if (obj == null) return;
            if (obj is GameObject go)       _resolver.InjectGameObject(go);
            else if (obj is Component comp) _resolver.InjectGameObject(comp.gameObject);
        }
    }
}
