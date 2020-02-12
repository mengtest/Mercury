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

        RegisterManager.Register(EntityEntry.Create().SetRegisterName(Consts.EntityWoodMan).Build());
        RegisterManager.Register(EntityEntry.Create().SetRegisterName(Consts.EntityFlightProp).Build());
        RegisterManager.Register(EntityEntry.Create()
            .SetRegisterName(Consts.EntityRaceter)
            .AddDependEntry(Consts.SkillRaceterShadowStrike)
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
        BuffFactory.Instance.Register(new BuffHeal());
        BuffFactory.Instance.Register(new BuffWindMark());

        nextSceneEntities = new List<AssetLocation>
        {
            Consts.EntityWoodMan,
            Consts.EntityRaceter
        };
    }
}