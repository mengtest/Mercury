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

    [NonSerialized] public List<AssetLocation> nextSceneEntities = new List<AssetLocation>
    {
        Consts.EntityWoodMan,
        Consts.EntityRaceter
    };

    protected override void OnAwake()
    {
        base.OnAwake();
        UIManager.Instance.Init();
        UIManager.Instance.ShowLoadPanel(0);
        BundleManager.Instance.Init(() => UIManager.Instance.HideLoadPanel());

        RegisterManager.Register(EntityEntry.Create().SetRegisterName(Consts.EntityWoodMan).Build());
        RegisterManager.Register(EntityEntry.Create().SetRegisterName(Consts.EntityFlightProp).Build());
        RegisterManager.Register(EntityEntry.Create()
            .SetRegisterName(Consts.EntityRaceter)
            .AddDependEntry(Consts.SkillRaceterShadowStrike)
            .AddDependEntry(Consts.SkillRaceterIaiAndSwallowFlip)
            .AddDependEntry(Consts.SkillRaceterBladeWave)
            .AddDependEntry(Consts.SkillRaceterFlashCut)
            .Build());

        RegisterManager.Register(SkillEntry.Create()
            .SetRegisterName(Consts.SkillNormal)
            .SetSkillType<NormalState>()
            .Build());
        RegisterManager.Register(SkillEntry.Create()
            .SetRegisterName(Consts.SkillStiffness)
            .SetSkillType<StiffnessState>()
            .Build());
        RegisterManager.Register(SkillEntry.Create()
            .SetRegisterName(Consts.SkillRaceterShadowStrike)
            .SetSkillType<SkillRaceterShadowStrike>()
            .AddDependAsset(Consts.SkillRaceterShadowStrike)
            .Build());
        RegisterManager.Register(SkillEntry.Create()
            .SetRegisterName(Consts.SkillRaceterIaiAndSwallowFlip)
            .SetSkillType<SkillRaceterIaiAndSwallowFlip>()
            .AddDependAsset(Consts.PrefabSkillRaceterSwallowFlip)
            .Build());
        RegisterManager.Register(SkillEntry.Create()
            .SetRegisterName(Consts.SkillRaceterBladeWave)
            .SetSkillType<SkillRaceterBladeWave>()
            .AddDependAsset(Consts.SkillRaceterBladeWave)
            .Build());
        RegisterManager.Register(SkillEntry.Create()
            .SetRegisterName(Consts.SkillRaceterFlashCut)
            .SetSkillType<SkillRaceterFlashCut>()
            .AddDependAsset(Consts.SkillRaceterFlashCut)
            .Build());

        RegisterManager.Register(new BuffHeal());
        RegisterManager.Register(new BuffWindMark());
    }
}