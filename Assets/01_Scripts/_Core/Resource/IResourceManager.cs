using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

using Cysharp.Threading.Tasks;

namespace RottenNoble.Core.Resource
{
    /// <summary>
    /// 리소스 로드 / 인스턴스 생성 공통 인터페이스
    /// </summary>
    public interface IResourceManager
    {
        UniTask<T>       LoadAsync<T>(string path, Action<string, float> onProgress = null) where T : Object;
        UniTask<List<T>> LoadsAsync<T>(List<string> paths, Action<string, float> onProgress = null) where T : Object;
        UniTask<List<T>> LoadByLabelAsync<T>(string label, Action<string, float> onProgress = null) where T : Object;

        UniTask<T>       CreateInstanceAsync<T>(string path, Transform parent = null) where T : Object;
        UniTask<List<T>> CreateInstancesAsync<T>(List<string> paths, Transform parent = null) where T : Object;

        void DeleteInstance(Object instanceObject);
        void Dispose();
    }
}
