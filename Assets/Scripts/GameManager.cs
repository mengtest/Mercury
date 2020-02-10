using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

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
    public List<AssetLocation> nextSceneElements;

    protected override void OnAwake()
    {
        base.OnAwake();

        UIManager.Instance.Init();
        UIManager.Instance.ShowLoadPanel(0);
        AssetManager.Instance.Init(() => UIManager.Instance.HideLoadPanel());

        RegisterManager.Register(new EntityEntry(Consts.EntityWoodMan, new[] {Consts.EntityWoodMan}));
        RegisterManager.Register(new EntityEntry(Consts.EntityFlightProp));
        RegisterManager.Register(new EntityEntry(Consts.EntityRaceter, new[] {Consts.EntityRaceter}));
        RegisterManager.Register(new NormalState());
        RegisterManager.Register(new StiffnessState());
        BuffFactory.Instance.Register(new BuffHeal());
        BuffFactory.Instance.Register(new BuffWindMark());

        nextSceneElements = new List<AssetLocation>
        {
            Consts.EntityWoodMan,
            Consts.EntityRaceter
        };
    }
}