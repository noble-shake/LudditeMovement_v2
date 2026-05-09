using System;
using System.Collections.Generic;
using UnityEngine;

using RottenNoble.Core.Resource;
using RottenNoble.Core.UI;

namespace RottenNoble.Core
{
    /// <summary>
    /// 씬 EntryPoint 공통 기반.
    /// Addressables로 생성한 GameObject 캐시를 추적하고 씬 종료 시 일괄 해제합니다.
    /// </summary>
    public class EntryPointBase : IDisposable
    {
        protected DataManager  dataManager;
        protected UINavigator  uiNavigator;

        readonly List<(ResourceType type, GameObject obj)> caches = new();
        bool disposed;

        protected void AddCache(ResourceType type, GameObject obj)
            => caches.Add((type, obj));

        protected void Cleanup()
        {
            for (int i = caches.Count - 1; i >= 0; i--)
            {
                var (t, obj) = caches[i];
                if (obj != null)
                    dataManager.Resource.DeleteInstance(t, obj);
                caches.RemoveAt(i);
            }
        }

        public void Dispose()
        {
            if (disposed) return;
            Cleanup();
            disposed = true;
        }
    }
}
