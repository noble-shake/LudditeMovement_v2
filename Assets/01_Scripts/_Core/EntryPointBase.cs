using System;
using System.Collections.Generic;
using UnityEngine;

using VContainer;

using RottenNoble.Core.Resource;
using RottenNoble.Core.UI;

namespace RottenNoble.Core
{
    /// <summary>
    /// 씬 EntryPoint 공통 기반.
    ///
    /// UINavigator에서 LoadAsync한 View는 자동으로 추적됩니다.
    /// Dispose 시 uiNavigator.DestroyAll()로 일괄 해제됩니다.
    ///
    /// UINavigator를 거치지 않고 직접 생성한 오브젝트는 AddCache(type, go)로 등록하세요.
    /// </summary>
    public class EntryPointBase : IDisposable
    {
        protected DataManager dataManager;
        protected UINavigator uiNavigator;

        [Inject]
        public void InjectInternal(DataManager dataManager, UINavigator uiNavigator)
        {
            this.dataManager = dataManager;
            this.uiNavigator = uiNavigator;
        }

        // UINavigator를 거치지 않고 직접 생성한 오브젝트
        readonly List<(ResourceType type, GameObject obj)> rawCaches = new();

        bool disposed;

        /// <summary>UINavigator를 거치지 않고 직접 생성한 오브젝트를 정리 목록에 등록합니다.</summary>
        protected void AddCache(ResourceType type, GameObject obj)
            => rawCaches.Add((type, obj));

        void Cleanup()
        {
            // UINavigator가 관리하는 모든 View 일괄 해제
            uiNavigator?.DestroyAll();

            // 직접 관리 오브젝트 해제
            for (int i = rawCaches.Count - 1; i >= 0; i--)
            {
                var (t, obj) = rawCaches[i];
                if (obj != null)
                    dataManager.ResourcePath.DeleteInstance(t, obj);
                rawCaches.RemoveAt(i);
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
