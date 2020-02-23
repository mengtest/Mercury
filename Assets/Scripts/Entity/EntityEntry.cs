using System;

namespace Mercury
{
    public class EntityEntry : RegistryEntryImpl<EntityEntry>
    {
        private readonly Type _entityType;
        private readonly Func<AssetLocation, Entity> _factory;

        public EntityEntry(AssetLocation registerName, Type type, Func<AssetLocation, Entity> factory)
        {
            SetRegisterName(registerName);
            _entityType = type;
            _factory = factory;
        }

        public Entity SpawnEntity() { return _factory(RegisterName); }

        public static void Init()
        {
            GameManager.Instance.Registries.QueryRegistry<EntityEntry>("entity", out var registry);
            registry.Register(new EntityEntry(new AssetLocation("mercury", "raceter"),
                typeof(EntityRaceter),
                id =>
                {
                    var r = GameManager.Instance.raceter;
                    var result = new EntityRaceter(id,
                        r,
                        new DamageData(),
                        new MotionData
                        {
                            moveSpeed = 2,
                            jumpSpeed = 1.5f,
                            groundDamping = 20f,
                            airDamping = 5f,
                            gravity = -25f
                        });
                    r.Player = result;
                    return result;
                }));
        }
    }
}