using System;
using System.Collections.Generic;

public class SkillEntry : IRegistryEntry
{
    /// <summary>
    /// 技能类类型
    /// </summary>
    public readonly Type skillType;

    /// <summary>
    /// 构造技能实例的工厂
    /// </summary>
    public readonly Func<Type, ISkillable, AbstractSkill> factory;

    /// <summary>
    /// 技能的注册名只用于注册，依赖的资源由DependAssets指出
    /// </summary>
    public AssetLocation RegisterName { get; }

    /// <summary>
    /// 该技能所需的资源
    /// </summary>
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

    public static Builder Create() { return new Builder(); }

    public class Builder
    {
        private AssetLocation _registerName;
        private List<AssetLocation> _dependAssets;
        private Func<Type, ISkillable, AbstractSkill> _constructor;
        private Type _skillType;

        internal Builder() { _dependAssets = new List<AssetLocation>(); }

        public Builder SetRegisterName(in AssetLocation registerName)
        {
            _registerName = registerName;
            return this;
        }

        public Builder AddDependAsset(in AssetLocation registerName)
        {
            _dependAssets.Add(registerName);
            return this;
        }

        public Builder SetSkillType<T>() where T : AbstractSkill
        {
            _skillType = typeof(T);
            return this;
        }

        public Builder SetConstructor(in Func<Type, ISkillable, AbstractSkill> constructor)
        {
            _constructor = constructor;
            return this;
        }

        public SkillEntry Build()
        {
            if (_skillType == null)
            {
                throw new ArgumentException();
            }

            if (_registerName.Equals(default))
            {
                throw new ArgumentException();
            }

            if (_constructor == null)
            {
                _constructor = EntityUtility.NormalSkillFactory;
            }

            return _dependAssets.Count == 0
                ? new SkillEntry(_registerName, null, _skillType, _constructor)
                : new SkillEntry(_registerName, _dependAssets.ToArray(), _skillType, _constructor);
        }
    }
}