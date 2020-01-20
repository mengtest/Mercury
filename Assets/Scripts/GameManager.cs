using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;

[Serializable]
public struct LevelAsset
{
    public string name;
    public AssetReference assRef;
    public bool avalible;
}

public class GameManager : MonoSingleton<GameManager>//TODO:解决canvas的依赖问题
{
    public GameObject canvas;
    public List<LevelAsset> levels;
    public SceneData nowScene;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(canvas);
        EntitySystemManager.Instance.Init();
    }

    public GameObject GetEffect(string key) { return nowScene.Pools[key].Get(); }

    public bool RecycleEffect(GameObject go)
    {
        return nowScene.Pools[go.name].Recycle(go);
    }

    public void DestroyEffect(string key)
    {
        var pool = nowScene.Pools[key];
        pool.TrimExcess(0);
    }

    public IEnumerator AsyncLoadScene(AssetReference scene, Action<SceneInstance> callback)
    {
        var req = Addressables.LoadSceneAsync(scene);
        UIManager.Instance.loadPanel.Active(() => req.PercentComplete);
        yield return req;
        callback?.Invoke(req.Result);
    }
}