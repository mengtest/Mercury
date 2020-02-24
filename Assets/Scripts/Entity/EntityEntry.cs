using System;

namespace Mercury
{
    /// <summary>
    /// 实体注册项
    /// </summary>
    public class EntityEntry : RegistryEntryImpl<EntityEntry>
    {
        /// <summary>
        /// 生成实体的工厂
        /// </summary>
        private readonly Func<AssetLocation, Entity> _factory;

        /// <param name="registerName">实体ID</param>
        /// <param name="factory">生成实体的工厂</param>
        public EntityEntry(AssetLocation registerName, Func<AssetLocation, Entity> factory)
        {
            SetRegisterName(registerName);
            _factory = factory;
        }

        /// <summary>
        /// 生成实体
        /// </summary>
        /// <returns></returns>
        public Entity SpawnEntity() { return _factory(RegisterName); }
    }
}