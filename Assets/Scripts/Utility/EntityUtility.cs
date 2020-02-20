using System;
using System.Reflection;
using System.Linq;
using UnityEngine;

public static class EntityUtility
{
    public static readonly Func<Type, ISkillable, AbstractSkill> NormalSkillFactory = (type, skillable) =>
    {
        var instance = Activator.CreateInstance(type, skillable); //TODO:是否需要把DI做成单独的类？
        if (!(instance is AbstractSkill skill))
        {
            throw new ArgumentException();
        }

        var regEntry = RegisterManager.Instance.Entries[skill.RegisterName];
        if (regEntry.DependAssets == null)
        {
            return skill;
        }

        var injected = skill
            .GetType()
            .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .Where(field => field.GetCustomAttribute(typeof(InjectAttribute)) is InjectAttribute)
            .ToArray();
        if (injected.Length == 0)
        {
            return skill;
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
                field.SetValue(skill, asset.Instantiate());
            }
            else if (field.FieldType == typeof(Asset))
            {
                field.SetValue(skill, asset);
            }
            else
            {
                Debug.LogError($"不支持注入的类型{field.FieldType.FullName}");
            }
        }

        return skill;
    };

    public static T GetSkill<T>(AssetLocation id, ISkillable user) where T : AbstractSkill
    {
        if (!(RegisterManager.Instance.Entries[id] is SkillEntry skill))
        {
            throw new ArgumentException();
        }

        var res = skill.factory.Invoke(skill.skillType, user) as T;
        res.Init();
        return res;
    }

    public static AbstractSkill GetSkill(AssetLocation id, ISkillable user) { return GetSkill<AbstractSkill>(id, user); }

    /// <summary>
    /// 获取一个Dot buff
    /// </summary>
    /// <param name="id">Buff的ID</param>
    /// <param name="entity">Buff来源</param>
    /// <param name="interval">触发间隔</param>
    /// <param name="triggerCount">触发次数</param>
    /// <param name="intensity">强度</param>
    /// <returns>Buff实例</returns>
    /// <exception cref="ArgumentException">不是Buff时触发</exception>
    public static BuffStack GetBuffDot(
        AssetLocation id,
        Entity entity,
        float interval,
        int triggerCount,
        int intensity)
    {
        if (!(RegisterManager.Instance.Entries[id] is AbstractBuff buff))
        {
            throw new ArgumentException();
        }

        return new BuffStack(buff, entity, interval, triggerCount, intensity);
    }

    /// <summary>
    /// 获取一个状态Buff
    /// </summary>
    /// <param name="id">Buff ID</param>
    /// <param name="entity">Buff来源</param>
    /// <param name="duration">持续时间</param>
    /// <param name="intensity">强度</param>
    /// <returns>Buff实例</returns>
    /// <exception cref="ArgumentException">不是Buff时触发</exception>
    public static BuffStack GetBuffState(
        AssetLocation id,
        Entity entity,
        float duration,
        int intensity)
    {
        if (!(RegisterManager.Instance.Entries[id] is AbstractBuff buff))
        {
            throw new ArgumentException();
        }

        return new BuffStack(buff, entity, duration, 1, intensity);
    }
}