using System;

public class SkillFactory
{
    public static readonly Func<Type, ISkillable, AbstractSkill> NormalFactory = (type, skillable) =>
        Activator.CreateInstance(type, skillable) as AbstractSkill;

    public static T Get<T>(AssetLocation id, ISkillable user) where T : AbstractSkill
    {
        if (RegisterManager.Instance.Entries[id] is SkillEntry skill)
        {
            return skill.factory.Invoke(skill.skillType, user) as T;
        }

        throw new ArgumentException();
    }
}