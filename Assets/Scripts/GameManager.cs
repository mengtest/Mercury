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
    public List<AssetLocation> nextSceneEntities;

    protected override void OnAwake()
    {
        base.OnAwake();

        UIManager.Instance.Init();
        UIManager.Instance.ShowLoadPanel(0);
        BundleManager.Instance.Init(() => UIManager.Instance.HideLoadPanel());

        RegisterManager.Register(new EntityEntry(Consts.EntityWoodMan, new[] {Consts.EntityWoodMan}));
        RegisterManager.Register(new EntityEntry(Consts.EntityFlightProp));
        RegisterManager.Register(new EntityEntry(Consts.EntityRaceter,
            new[] {Consts.EntityRaceter, Consts.SkillRaceterShadowStrike}));
        RegisterManager.Register(new SkillEntry(Consts.SkillNormal,
            null,
            typeof(NormalState),
            SkillFactory.Normal));
        RegisterManager.Register(new SkillEntry(Consts.SkillStiffness,
            null,
            typeof(StiffnessState),
            SkillFactory.Normal));
        RegisterManager.Register(new SkillEntry(Consts.SkillRaceterShadowStrike,
            new[] {Consts.SkillRaceterShadowStrike},
            typeof(SkillRaceterShadowStrike),
            SkillFactory.Normal));
        BuffFactory.Instance.Register(new BuffHeal());
        BuffFactory.Instance.Register(new BuffWindMark());

        nextSceneEntities = new List<AssetLocation>
        {
            Consts.EntityWoodMan,
            Consts.EntityRaceter
        };
    }
}