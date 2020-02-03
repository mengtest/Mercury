using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AssetManager : MonoSingleton<AssetManager>//暂时没啥用...
{
    private Queue<AsyncOperationHandle<GameObject>> _asyncQueue;
    private Coroutine _queryQueue;
    private AssetReference _r;

    protected override void OnAwake()
    {
        base.OnAwake();
        _asyncQueue = new Queue<AsyncOperationHandle<GameObject>>();
        _queryQueue = StartCoroutine(QueryAsyncQueue());
    }

    private IEnumerator QueryAsyncQueue()
    {
        if (_asyncQueue.Count == 0)
        {
            yield return null;
        }

        var handle = _asyncQueue.Peek();
        if (handle.IsDone)
        {
            _asyncQueue.Dequeue();
        }

        yield return null;
    }

    public void InstantiateGameObjectAsync(in AssetLocation assetLocation)
    {
        
    }
}