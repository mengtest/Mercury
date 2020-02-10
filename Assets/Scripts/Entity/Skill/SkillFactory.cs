using System;

public class SkillFactory
{
    public static SkillStack Get(AssetLocation id, ISkillable user, object properties = null)
    {
        if (RegisterManager.Instance.Entries[id] is AbstractSkill skill)
        {
            return new SkillStack(user, skill, properties);
        }

        throw new ArgumentException();
    }
}