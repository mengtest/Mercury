using System;

public class SkillFactory
{
    public static readonly Func<Type, ISkillable, AbstractSkill> Normal = (type, skillable) =>
        Activator.CreateInstance(type, skillable) as AbstractSkill;

    public static T Get<T>(AssetLocation id, ISkillable user) where T : AbstractSkill
    {
        if (!(RegisterManager.Instance.Entries[id] is SkillEntry skill))
        {
            throw new ArgumentException();
        }

        var res = skill.factory.Invoke(skill.skillType, user) as T;
        res.Init();
        return res;
    }
}