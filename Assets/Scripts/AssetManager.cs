using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Mercury
{
    /// <summary>
    /// 资源管理
    /// </summary>
    public class AssetManager
    {
        private readonly GameManager _gameManager;
        private readonly Dictionary<string, GameObject> _activePrefabs;
        private Queue<AsyncOperationHandle> _queries;
        private Dictionary<string, AsyncOperationHandle> _loading;
        private Action _completeCallback;

        public AssetManager(GameManager gameManager)
        {
            _gameManager = gameManager;
            _activePrefabs = new Dictionary<string, GameObject>();
        }

        /// <summary>
        /// 准备加载资源
        /// </summary>
        public void ReadyToLoad()
        {
            _gameManager.CheckState(GameState.Waiting, "只有waiting状态才能准备加载");
            _gameManager.SetGameState(GameState.ReadyToLoad);
            _queries = new Queue<AsyncOperationHandle>();
            _loading = new Dictionary<string, AsyncOperationHandle>();
        }

        /// <summary>
        /// 设置加载完毕后的回调
        /// </summary>
        public void SetCompleteCallback(Action callback)
        {
            _gameManager.CheckState(GameState.ReadyToLoad, "只有ReadyToLoad状态才能添加完成异步的回调");
            _completeCallback = callback;
        }

        /// <summary>
        /// 添加加载请求
        /// </summary>
        public void AddRequest(in AsyncOperationHandle addressableHandle)
        {
            _gameManager.CheckState(GameState.ReadyToLoad, "只有ReadyToLoad才能添加加载委托");
            _queries.Enqueue(addressableHandle);
        }

        /// <summary>
        /// 添加加载请求
        /// </summary>
        public void AddRequest(string address, Action<AsyncOperationHandle<GameObject>> callback)
        {
            _gameManager.CheckState(GameState.ReadyToLoad, "只有ReadyToLoad才能添加加载委托");
            var fullName = address;
            if (_loading.TryGetValue(fullName, out var handle))
            {
                var convert = handle.Convert<GameObject>();
                convert.Completed += callback;
                _loading[fullName] = convert;
                return;
            }

            var key = address;
            var req = Addressables.LoadAssetAsync<GameObject>(key);
            req.Completed += q =>
            {
                var l = q.Result;
                if (l == null)
                {
                    Debug.LogError($"无法找到资源{fullName}");
                    return;
                }

                _activePrefabs.Add(fullName, l);
            };
            req.Completed += callback;
            AddRequest(req);
            _loading[fullName] = req;
        }

        /// <summary>
        /// 添加加载请求
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <param name="callback"></param>
        public void AddRequest(string type, AssetLocation id, Action<AsyncOperationHandle<GameObject>> callback) { AddRequest($"{id.Label}.{type}.{id.Name}", callback); }

        /// <summary>
        /// 开始加载
        /// </summary>
        public void StartLoad()
        {
            _gameManager.CheckState(GameState.ReadyToLoad, "只有ReadyToLoad状态才能开始加载资源");
            _gameManager.SetGameState(GameState.Loading);
            _gameManager.StartCoroutine(LoadCoroutine());
        }

        private IEnumerator LoadCoroutine()
        {
            while (true)
            {
                if (_queries.Count == 0)
                {
                    try
                    {
                        _completeCallback?.Invoke();
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                    finally
                    {
                        _loading = null;
                        _queries = null;
                        _completeCallback = null;
                        _gameManager.SetGameState(GameState.Waiting);
                    }

                    break;
                }

                if (_queries.Peek().IsDone)
                {
                    _queries.Dequeue();
                }

                yield return null;
            }
        }

        /// <summary>
        /// 转换id
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private static string ConvertId(string type, AssetLocation id) { return $"{id.Label}.{type}.{id.Name}"; }

        /// <summary>
        /// 获取预制体
        /// </summary>
        /// <param name="type">预制体类型</param>
        /// <param name="id">预制体id</param>
        public GameObject GetPrefab(string type, AssetLocation id)
        {
            var fullName = ConvertId(type, id);
            if (!_activePrefabs.TryGetValue(fullName, out var result))
            {
                throw new ArgumentException($"无法找到预制体{fullName}");
            }

            return result;
        }

        /// <summary>
        /// 释放所有已加载预制体
        /// </summary>
        public void ReleaseActivePrefab() { _activePrefabs.Clear(); }
    }
}