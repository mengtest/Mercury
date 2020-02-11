using System;
using System.Collections.Generic;

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
        BundleManager.Instance.Init(() => UIManager.Instance.HideLoadPanel());

        RegisterManager.Register(new EntityEntry(Consts.EntityWoodMan, new[] {Consts.EntityWoodMan}));
        RegisterManager.Register(new EntityEntry(Consts.EntityFlightProp));
        RegisterManager.Register(new EntityEntry(Consts.EntityRaceter, new[] {Consts.EntityRaceter}));
        RegisterManager.Register(new SkillEntry(Consts.SkillNormal,
            null,
            typeof(NormalState),
            SkillFactory.NormalFactory));
        RegisterManager.Register(new SkillEntry(Consts.SkillStiffness,
            null,
            typeof(StiffnessState),
            SkillFactory.NormalFactory));
        BuffFactory.Instance.Register(new BuffHeal());
        BuffFactory.Instance.Register(new BuffWindMark());

        nextSceneElements = new List<AssetLocation>
        {
            Consts.EntityWoodMan,
            Consts.EntityRaceter
        };
    }
}