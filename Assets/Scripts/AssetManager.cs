using System;
using System.Collections.Generic;
using UnityEngine;

public class Asset
{
    public readonly string name;

    /// <summary>
    /// 你不应该直接使用这个
    /// </summary>
    public UnityEngine.Object res;

    public AssetBundleRequest request;
    internal HashSet<GameObject> goRef;
    internal int refCount;

    public Asset(string resName) { name = resName; }

    public int RefCount
    {
        get => refCount;
        set
        {
            if (value < 0)
            {
                throw new InvalidOperationException($"引用计数不能小于0");
            }

            refCount = value;
        }
    }

    public T Get<T>() where T : UnityEngine.Object
    {
        RefCount += 1;
        return res as T;
    }

    public GameObject Instantiate()
    {
        if (!(res is GameObject go))
        {
            throw new InvalidOperationException();
        }

        RefCount += 1;
        if (goRef == null)
        {
            goRef = new HashSet<GameObject>();
        }

        var g = UnityEngine.Object.Instantiate(go);
        goRef.Add(g);
        return g;
    }

    public void Recycle(ref GameObject asset)
    {
        if (asset == res)
        {
            RefCount -= 1;
            asset = null;
        }
        else
        {
            throw new InvalidOperationException();
        }
    }

    public void Destroy(GameObject instance)
    {
        if (goRef.Contains(instance))
        {
            RefCount -= 1;
            UnityEngine.Object.Destroy(instance);
        }
        else
        {
            throw new InvalidOperationException();
        }
    }
}

public class AssetManager : Singleton<AssetManager>
{
    private readonly Dictionary<string, Asset> _actRes;

    public IReadOnlyDictionary<string, Asset> LoadedAssets => _actRes;

    private AssetManager() { _actRes = new Dictionary<string, Asset>(); }

    public static void ReadyToLoad(Action onComplete = null) { BundleManager.Instance.ReadyToLoad(onComplete); }

    public static void AddRequest<T>(AssetLocation location, Action<Asset> callback = null) where T : UnityEngine.Object
    {
        var name = location.ToString();
        var q = BundleManager.Instance.AddRequest<T>(location,
            req =>
            {
                if (!(req is AssetBundleRequest bundle))
                {
                    throw new InvalidOperationException();
                }

                if (bundle.asset is T obj)
                {
                    var asset = Instance._actRes[name];
                    asset.request = null;
                    asset.res = obj;
                    callback?.Invoke(asset);
                }
                else
                {
                    throw new ArgumentException($"请求参数是{typeof(T)},但资源类型是{bundle.asset.GetType()}");
                }
            });
        Instance._actRes.Add(name, new Asset(name) {request = q});
    }

    public static void StartLoad() { BundleManager.Instance.StartLoad(); }

    public static void DestroyActiveResources()
    {
        foreach (var asset in Instance._actRes.Values)
        {
            if (asset.res is GameObject && asset.goRef != null)
            {
                foreach (var o in asset.goRef)
                {
                    UnityEngine.Object.Destroy(o);
                }

                asset.refCount = 0;
            }

            if (asset.RefCount != 0)
            {
                throw new InvalidOperationException();
            }
        }
    }
}