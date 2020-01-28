using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.UI;
using Random = Unity.Mathematics.Random;

[Serializable]
public struct LevelAsset
{
    public string name;
    public AssetReference assRef;
    public bool avalible;
}

public class GameManager : MonoSingleton<GameManager>
{
    public GameObject canvas;
    public List<LevelAsset> levels;
    public SceneData nowScene;
    public Random Rand { get; } = new Random((uint) DateTime.Now.Ticks);

    protected override void Awake()
    {
        base.Awake();
#if UNITY_EDITOR
        canvas = GameObject.Find("Canvas");
        if (!canvas)
        {
            var c = new GameObject("Canvas");
            c.AddComponent<Canvas>();
            c.AddComponent<CanvasScaler>();
            c.AddComponent<GraphicRaycaster>();
            canvas = c;
        }
#endif
        DontDestroyOnLoad(canvas);
    }

    public GameObject GetEffect(string key) { return nowScene.Pools[key].Get(); }

    public bool RecycleEffect(GameObject go) { return nowScene.Pools[go.name].Recycle(go); }

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