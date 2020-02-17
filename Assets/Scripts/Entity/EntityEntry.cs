using System;
using System.Collections.Generic;

public class EntityEntry : IRegistryEntry
{
    /// <summary>
    /// 实体的注册名用于寻找对应预制体，所以一个注册名对应一个预制体
    /// </summary>
    public AssetLocation RegisterName { get; }

    /// <summary>
    /// 实体的依赖资源表示：注册项，例如的技能、Buff之类，而不是资源
    /// </summary>
    public IReadOnlyList<AssetLocation> DependAssets { get; }

    public Action<Entity> OnEntityAwake { get; } //TODO:使用事件总线
    public Action<Entity> OnEntityStart { get; }

    public EntityEntry(
        AssetLocation registerName,
        IReadOnlyList<AssetLocation> dependAssets = null,
        Action<Entity> onAwake = null,
        Action<Entity> onStart = null)
    {
        RegisterName = registerName;
        DependAssets = dependAssets;
        OnEntityAwake = onAwake;
        OnEntityStart = onStart;
    }

    public static Builder Create() { return new Builder(); }

    public class Builder
    {
        private AssetLocation _registerName;
        private List<AssetLocation> _dependRegistryEntries;
        private Action<Entity> _awake;
        private Action<Entity> _start;

        internal Builder() { _dependRegistryEntries = new List<AssetLocation>(); }

        public Builder SetRegisterName(in AssetLocation registerName)
        {
            _registerName = registerName;
            return this;
        }

        public Builder AddDependEntry(in AssetLocation registerName)
        {
            _dependRegistryEntries.Add(registerName);
            return this;
        }

        public Builder SetAwakeEvent(in Action<Entity> onAwake)
        {
            _awake = onAwake;
            return this;
        }

        public Builder SetStartEvent(in Action<Entity> onStart)
        {
            _start = onStart;
            return this;
        }

        public EntityEntry Build()
        {
            if (_registerName == null)
            {
                throw new ArgumentException();
            }

            return _dependRegistryEntries.Count == 0
                ? new EntityEntry(_registerName, null, _awake, _start)
                : new EntityEntry(_registerName, _dependRegistryEntries.ToArray(), _awake, _start);
        }
    }
}