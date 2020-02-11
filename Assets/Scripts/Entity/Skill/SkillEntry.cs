using System;
using System.Collections.Generic;

public class SkillEntry : IRegistryEntry
{
    public readonly Type skillType;
    public readonly Func<Type, ISkillable, AbstractSkill> factory;
    public AssetLocation RegisterName { get; }
    public IReadOnlyList<AssetLocation> DependAssets { get; }

    public SkillEntry(
        AssetLocation location,
        IReadOnlyList<AssetLocation> dependAssets,
        Type type,
        Func<Type, ISkillable, AbstractSkill> factory)
    {
        RegisterName = location;
        DependAssets = dependAssets;
        skillType = type;
        this.factory = factory;
    }
}