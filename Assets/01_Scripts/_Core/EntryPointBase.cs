using System;
using System.Collections.Generic;
using UnityEngine;

using RottenNoble.Core.Resource;

namespace RottenNoble.Core
{
    /// <summary>
    /// 씬 EntryPoint 공통 기반.
    /// Addressables로 생성한 GameObject 캐시를 추적하고 씬 종료 시 일괄 해제합니다.
    /// </summary>
    public class EntryPointBase : IDisposable
    {
        protected ResourceFactory resourceFactory;

        readonly List<(ResourceType type, GameObject obj)> _caches = new();
        bool _disposed;

        protected void AddCache(ResourceType type, GameObject obj)
            => _caches.Add((type, obj));

        protected void Cleanup()
        {
            for (int i = _caches.Count - 1; i >= 0; i--)
            {
                var (t, obj) = _caches[i];
                if (obj != null)
                    resourceFactory.DeleteInstance(t, obj);
                _caches.RemoveAt(i);
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            Cleanup();
            _disposed = true;
        }
    }
}
