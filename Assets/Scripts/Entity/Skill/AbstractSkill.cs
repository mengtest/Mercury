using System.Linq;
using System.Reflection;
using UnityEngine;

/// <summary>
/// 抽象技能类
/// </summary>
public abstract class AbstractSkill : IFSMState
{
    /// <summary>
    /// 注册ID
    /// </summary>
    public abstract AssetLocation RegisterName { get; }

    /// <summary>
    /// 技能使用者
    /// </summary>
    public ISkillable User { get; }

    /// <summary>
    /// 技能的有限状态机
    /// </summary>
    public FSMSystem System { get; }

    public AbstractSkill(ISkillable user)
    {
        User = user;
        System = user.SkillFsmSystem;
    }

    /// <summary>
    /// 初始化，在实例化对象后调用
    /// </summary>
    public virtual void Init()
    {
        var regEntry = RegisterManager.Instance.Entries[RegisterName];
        if (regEntry.DependAssets == null)
        {
            return;
        }

        var injected = GetType()
            .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .Where(field => field.GetCustomAttribute(typeof(InjectAttribute)) is InjectAttribute)
            .ToArray();
        if (injected.Length == 0)
        {
            return;
        }

        if (injected.Length != regEntry.DependAssets.Count)
        {
            Debug.LogWarning("存在未使用却注册的资源，或注册资源数量与需注入资源数量不符");
        }

        for (var i = 0; i < injected.Length; i++)
        {
            var field = injected[i];
            var asset = AssetManager.Instance.LoadedAssets[regEntry.DependAssets[i].ToString()];
            if (field.FieldType == typeof(GameObject))
            {
                field.SetValue(this, asset.Instantiate());
            }
            else if (field.FieldType == typeof(Asset))
            {
                field.SetValue(this, asset);
            }
            else
            {
                Debug.LogError($"不支持注入的类型{field.FieldType.FullName}");
            }
        }
    }

    /// <summary>
    /// 是否可以使用该技能
    /// </summary>
    public abstract bool CanEnter();

    /// <summary>
    /// 当使用技能时触发
    /// </summary>
    public abstract void OnEnter();

    /// <summary>
    /// 技能使用中触发，每帧调用
    /// </summary>
    public abstract void OnUpdate();

    /// <summary>
    /// 技能出发完毕，离开时触发
    /// </summary>
    public abstract void OnLeave();
}