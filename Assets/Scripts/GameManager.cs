using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct LevelAsset
{
    public string name;
    public bool avalible;
}

public class GameManager : MonoSingleton<GameManager>
{
    public List<LevelAsset> levels;
    public SceneData nowScene;

    [NonSerialized] public List<AssetLocation> nextSceneEntities = new List<AssetLocation>
    {
        Consts.EntityWoodMan,
        Consts.EntityRaceter
    };

    protected override void OnAwake()
    {
        base.OnAwake();
        EventManager.Instance.Init();
        IoCContainer.Instance.Init();
        UIManager.Instance.Init();
        UIManager.Instance.ShowLoadPanel(0);
        BundleManager.Instance.Init(() => UIManager.Instance.HideLoadPanel());
        RegisterManager.Instance.Init();
    }
}