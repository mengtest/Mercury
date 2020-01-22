using System;
using System.Collections;
using System.Collections.Generic;
using Guirao.UltimateTextDamage;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;

public class SceneData : MonoBehaviour
{
    public Transform poolTrans;
    public List<AssetReference> assets;
    public List<AssetReference> pools;
    public bool whetherWaitForPools;
    public UltimateTextDamageManager textDamage;
    public EventSystem eventSystem;
    protected Dictionary<string, ObjectPool<GameObject>> _pools;
    protected bool _isPoolsDone;
    protected bool _isStartLoadAssets;
    protected bool _isAllDone;

    public IReadOnlyDictionary<string, ObjectPool<GameObject>> Pools => _pools;

    private void Start()
    {
        GameManager.Instance.nowScene = this;
        UIManager.Instance.textDamageManager = textDamage;
        _pools = new Dictionary<string, ObjectPool<GameObject>>(pools.Count);
        StartCoroutine(LoadElementsAsync<GameObject>(pools,
            () =>
            {
                foreach (var e in pools)
                {
                    var obj = e.Asset as GameObject;
                    var go = new GameObject(obj.name + "_Pool");
                    go.transform.parent = poolTrans;
                    var sePool = new ObjectPool<GameObject>(obj,
                        (o) =>
                        {
                            var res = Instantiate(o).Hide();
                            res.name = o.name;
                            res.transform.parent = go.transform;
                            return res;
                        },
                        1);
                    sePool.Factory.OnDestruct += Destroy;
                    sePool.OnGet += ob => ob.Show();
                    sePool.OnRecycle += ob => ob.Hide();
                    _pools.Add(obj.name, sePool);
                }

                _isPoolsDone = true;
            }));
        if (!whetherWaitForPools)
        {
            LoadAssets();
        }
    }

    private void Update()
    {
        if (_isAllDone)
        {
            return;
        }

        if (!whetherWaitForPools || !_isPoolsDone || _isStartLoadAssets)
        {
            return;
        }

        LoadAssets();
        _isStartLoadAssets = true;
    }

    public static IEnumerator LoadElementsAsync<T>(IList<AssetReference> assetList, Action onComplete)
        where T : UnityEngine.Object
    {
        UIManager.Instance.ShowLoadPanel(() => 0);
        foreach (var asset in assetList)
        {
            asset.LoadAssetAsync<T>();
        }

        yield return null;
        var complete = 0;
        var loading = 0;
        while (true)
        {
            foreach (var asset in assetList)
            {
                if (asset.IsDone)
                {
                    complete += 1;
                }
                else
                {
                    loading += 1;
                }
            }

            UIManager.Instance.ShowLoadPanel((float) complete / assetList.Count);
            if (assetList.Count == complete && loading == 0)
            {
                onComplete?.Invoke();
                yield break;
            }

            complete = 0;
            loading = 0;
            yield return null;
        }
    }

    public virtual void LoadAssets()
    {
        StartCoroutine(LoadElementsAsync<GameObject>(assets,
            () =>
            {
                UIManager.Instance.ShowLoadPanel(1);
                foreach (var asset in assets)
                {
                    Instantiate(asset.Asset); //TODO:可能每个场景都不一样
                }

                UIManager.Instance.HideLoadPanel();
                _isAllDone = true;
            }));
    }
}