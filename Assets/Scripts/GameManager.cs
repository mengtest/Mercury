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
        var asmData = GetAsmData();
        EventManager.Instance.Init(asmData);
        IoCContainer.Instance.Init();
        UIManager.Instance.Init();
        UIManager.Instance.ShowLoadPanel(0);
        BundleManager.Instance.Init(() => UIManager.Instance.HideLoadPanel());
        RegisterManager.Instance.Init(asmData);
    }

    private IReadOnlyDictionary<Type, List<Type>> GetAsmData()
    {
        var data = new Dictionary<Type, List<Type>>();
        foreach (var type in typeof(GameManager).Assembly.GetTypes())
        {
            var attr = type.GetCustomAttributes(true);
            foreach (var a in attr)
            {
                if (data.TryGetValue(a.GetType(), out var clz))
                {
                    clz.Add(type);
                }
                else
                {
                    data.Add(a.GetType(), new List<Type> {type});
                }
            }
        }

        return data;
    }
}