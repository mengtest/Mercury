﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;

[Serializable]
public struct LevelAsset
{
    public string name;
    public AssetReference assRef;
    public bool avalible;
}

public class GameManager : MonoSingleton<GameManager>
{
    public List<LevelAsset> levels;
    public SceneData nowScene;

    protected override void OnAwake()
    {
        base.OnAwake();
        BuffFactory.Instance.Register(new BuffHeal());
        BuffFactory.Instance.Register(new BuffWindMark());
    }

    public IEnumerator AsyncLoadScene(AssetReference scene, Action<SceneInstance> callback)
    {
        var req = Addressables.LoadSceneAsync(scene);
        UIManager.Instance.loadPanel.Active(() => req.PercentComplete);
        yield return req;
        callback?.Invoke(req.Result);
    }
}