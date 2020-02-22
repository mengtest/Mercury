using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;

public static class EntityUtility
{
    public static readonly Func<Type, ISkillable, AbstractSkill> NormalSkillFactory = (type, skillable) =>
    {
        var instance = Activator.CreateInstance(type, skillable);
        if (!(instance is AbstractSkill skill))
        {
            throw new ArgumentException();
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
    public static BuffStack GetBuffDot(AssetLocation id, Entity entity, float interval, int triggerCount, int intensity)
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
    public static BuffStack GetBuffState(AssetLocation id, Entity entity, float duration, int intensity)
    {
        if (!(RegisterManager.Instance.Entries[id] is AbstractBuff buff))
        {
            throw new ArgumentException();
        }

        return new BuffStack(buff, entity, duration, 1, intensity);
    }

    public static Entity SpawnEntity(AssetLocation id)
    {
        var gameObject = AssetManager.Instance.LoadedAssets[id.ToString()].Instantiate();
        var entity = gameObject.GetComponent<Entity>();
        if (!entity)
        {
            entity = gameObject.GetComponentInChildren<Entity>();
        }

        if (!entity)
        {
            throw new ArgumentException("wtf");
        }

        entity.Init(id);
        return entity;
    }

    public static EntityFlightProp SpawnFlightProp(GameObject go)
    {
        var flight = go.AddComponent<EntityFlightProp>();
        flight.Init(Consts.EntityFlightProp);
        return flight;
    }
}