using System;

public static class EntityUtility
{
    public static readonly Func<Type, ISkillable, AbstractSkill> NormalSkillFactory = (type, skillable) =>
        Activator.CreateInstance(type, skillable) as AbstractSkill;

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