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
    /// 리소스 정리 방식 두 가지:
    ///   AddUICache&lt;T&gt;()         — UINavigator 캐시 View 등록 (Destroy&lt;T&gt; 자동 호출)
    ///   AddCache(type, go)      — UINavigator 비관리 오브젝트 수동 등록
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

        // UINavigator 관리 View 정리 액션
        readonly List<Action> uiCleanupActions = new();

        // 직접 관리 오브젝트 (UINavigator를 거치지 않은 경우)
        readonly List<(ResourceType type, GameObject obj)> rawCaches = new();

        bool disposed;

        /// <summary>UINavigator가 관리하는 View를 정리 목록에 등록합니다.</summary>
        protected void AddUICache<T>() where T : ViewBase
            => uiCleanupActions.Add(() => uiNavigator.Destroy<T>());

        /// <summary>UINavigator를 거치지 않고 직접 생성한 오브젝트를 정리 목록에 등록합니다.</summary>
        protected void AddCache(ResourceType type, GameObject obj)
            => rawCaches.Add((type, obj));

        protected void Cleanup()
        {
            foreach (var action in uiCleanupActions)
                action();
            uiCleanupActions.Clear();

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
