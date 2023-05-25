using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Infrastructure.AssetManagement
{
    public class  AssetProvider : IAssets
    {
        private readonly Dictionary<string, AsyncOperationHandle> _completedCache = new();
        private readonly Dictionary<string, List<AsyncOperationHandle>> _handles = new();

        private readonly Dictionary<string, Object> _resourcesPrefabCache = new();

        public void Initialize()
        {
            Addressables.InitializeAsync();
        }

        public async Task<T> Load<T>(AssetReference assetReference) where T : class
        {
            if (_completedCache.TryGetValue(assetReference.AssetGUID, out var completedHandle))
            {
                return completedHandle.Result as T;
            }

            return await RunWithCacheOnComplete(
                Addressables.LoadAssetAsync<T>(assetReference),
                cacheKey: assetReference.AssetGUID);
        }

        public async Task<T> Load<T>(string address) where T : class
        {
            if (_completedCache.TryGetValue(address, out var completedHandle))
            {
                return completedHandle.Result as T;
            }

            return await RunWithCacheOnComplete(
                Addressables.LoadAssetAsync<T>(address),
                cacheKey: address);
        }

        public Task<GameObject> Instantiate(string address)
        {
            return Addressables.InstantiateAsync(address).Task;
        }

        public Task<GameObject> Instantiate(string address, Vector3 at)
        {
            return Addressables.InstantiateAsync(address, at, Quaternion.identity).Task;
        }

        public T InstantiateNonAsync<T>(string path) where T : Object
        {
            T prefab = LoadNonAsync<T>(path);
            T reference = Object.Instantiate(prefab);
            
            return reference;
        }

        private T LoadNonAsync<T>(string path) where T : Object
        {
            if (!_resourcesPrefabCache.ContainsKey(path))
            {
                _resourcesPrefabCache[path] = Resources.Load<T>(path);
            }

            return _resourcesPrefabCache[path] as T;
        }

        public void CleanUp()
        {
            foreach (List<AsyncOperationHandle> resourceHandles in _handles.Values)
            foreach (AsyncOperationHandle handle in resourceHandles)
            {
                Addressables.Release(handle);
            }
            
            _completedCache.Clear();
            _handles.Clear();
        }

        private async Task<T> RunWithCacheOnComplete<T>(AsyncOperationHandle<T> handle, string cacheKey) where T : class
        {
            handle.Completed += completeHandle =>
            {
                _completedCache[cacheKey] = completeHandle;
            };

            AddHandle(cacheKey, handle);

            return await handle.Task;
        }

        private void AddHandle<T>(string key, AsyncOperationHandle<T> handle) where T : class
        {
            if (!_handles.TryGetValue(key, out List<AsyncOperationHandle> resourceHandles))
            {
                resourceHandles = new List<AsyncOperationHandle>();
                _handles[key] = resourceHandles;
            }
            
            resourceHandles.Add(handle);
        }
    }
}