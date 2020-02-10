using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public enum LoadState
{
    Init,
    Loading,
    Sleep,
    Ready
}

public class AssetManager : MonoSingleton<AssetManager>
{
    public LoadState state = LoadState.Init;
    private Dictionary<string, AssetBundle> _abs;
    private List<AsyncOperation> _loadReq;

    private Action _onLoadComplete;

    protected override void OnUpdate()
    {
        switch (state)
        {
            case LoadState.Loading:
            {
                Loading();
                break;
            }
            case LoadState.Init:
            {
                Loading();
                break;
            }
            case LoadState.Sleep:
                return;
            case LoadState.Ready:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void Init(Action onComplete)
    {
        CheckState(LoadState.Init);
        var asRoot = new DirectoryInfo(Application.streamingAssetsPath);
        if (!asRoot.Exists)
        {
            throw new InvalidOperationException();
        }

        var files = asRoot.GetFiles("*.bundle");
        _abs = new Dictionary<string, AssetBundle>(files.Length);
        _loadReq = new List<AsyncOperation>(files.Length);
        foreach (var file in files)
        {
            var req = AssetBundle.LoadFromFileAsync(file.FullName);
            req.completed += ab =>
            {
                var prefix = file.Name.Split('.');
                _abs.Add(prefix[0], req.assetBundle);
                Debug.Log($"loaded {prefix[0]}");
            };
            _loadReq.Add(req);
        }

        _onLoadComplete += () => Debug.Log("Complete!");
        _onLoadComplete += onComplete;
    }

    public void ReadyToLoad(Action onComplete = null)
    {
        CheckState(LoadState.Sleep, "只有Sleep可以准备加载");
        state = LoadState.Ready;
        _onLoadComplete = onComplete;
        _loadReq = new List<AsyncOperation>();
    }

    public void AddRequest(AsyncOperation request)
    {
        CheckState(LoadState.Ready);
        _loadReq.Add(request);
    }

    public void AddRequest<T>(AssetLocation assetLocation, Action<AsyncOperation> callback = null)
        where T : UnityEngine.Object
    {
        CheckState(LoadState.Ready);
        if (_abs.TryGetValue(assetLocation.label, out var ab))
        {
            var req = ab.LoadAssetAsync<T>(assetLocation.GetAssetName());
            if (callback != null)
            {
                req.completed += callback;
            }

            _loadReq.Add(req);
        }
        else
        {
            throw new ArgumentException(assetLocation.ToString());
        }
    }

    public void StartLoad()
    {
        CheckState(LoadState.Ready);
        state = LoadState.Loading;
    }

    private void Loading()
    {
        var done = true;
        foreach (var op in _loadReq)
        {
            if (!op.isDone)
            {
                done = false;
                break;
            }
        }

        if (done)
        {
            _onLoadComplete?.Invoke();
            _onLoadComplete = null;
            _loadReq = null;
            state = LoadState.Sleep;
        }
    }

    private void CheckState(LoadState targetState, string msg = "")
    {
        if (state != targetState)
        {
            throw new InvalidOperationException($"当前状态:{targetState},{msg}");
        }
    }
}