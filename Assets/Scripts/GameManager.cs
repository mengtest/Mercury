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
        RegisterManager.Instance.AddRegistryType(typeof(AbstractBuff), (type, _) => (IRegistryEntry) Activator.CreateInstance(type, true));
        RegisterManager.Instance.AddRegistryType(typeof(Entity), EntityEntry.AutoRegisterFunc);
        RegisterManager.Instance.AddRegistryType(typeof(AbstractSkill), SkillEntry.AutoRegisterFunc);
        RegisterManager.Instance.Init();
    }
}