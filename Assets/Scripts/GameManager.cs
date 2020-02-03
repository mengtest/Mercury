using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.UI;

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

    protected override void OnAwake()
    {
        base.OnAwake();
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

    [Obsolete("不再使用对象池的方式储存技能特效")]
    public GameObject GetEffect(string key) { return nowScene.Pools[key].Get(); }

    [Obsolete("不再使用对象池的方式储存技能特效")]
    public bool RecycleEffect(GameObject go) { return nowScene.Pools[go.name].Recycle(go); }

    [Obsolete("不再使用对象池的方式储存技能特效")]
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