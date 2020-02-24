using System;
using System.Collections.Generic;

namespace Mercury
{
    /// <summary>
    /// 玩家
    /// </summary>
    public class EntityPlayer : Entity, ISystemOwner, IAttackable, IMovable
    {
        private readonly Dictionary<Type, IEntitySystem> _entitySystems;

        public EntityPlayer(
            AssetLocation id,
            DamageData damageData,
            IDamageCompute damageCompute,
            MotionData motionData,
            IMotionCompute moveCompute,
            IMoveSystem moveSystem) : base(id)
        {
            _entitySystems = new Dictionary<Type, IEntitySystem>();
            DamageRawData = damageData;
            DamageCompute = damageCompute;
            MotionRawData = motionData;
            MoveCompute = moveCompute;
            MoveSystem = moveSystem;
            AddComponent(DamageRawData);
            AddComponent(MotionRawData);
        }

        public virtual void OnUpdate()
        {
            foreach (var system in _entitySystems.Values)
            {
                system.OnUpdate();
            }
        }

        public DamageData DamageRawData { get; }
        public IDamageCompute DamageCompute { get; }

        public Damage CalculateDamage(float coe, DamageType type)
        {
            //TODO:DamageSystem
            return new Damage();
        }

        public Damage DealDamage(in Damage damage, IAttackable target)
        {
            GameManager.Instance.EventBus.Publish(this, new EntityAttackEvent(this, target, damage));
            return damage;
        }

        public void UnderAttack(in Damage damage) { }

        public MotionData MotionRawData { get; }
        public IMotionCompute MoveCompute { get; }
        public IMoveSystem MoveSystem { get; }

        public void AddSystem<T>(T system) where T : class, IEntitySystem { _entitySystems.Add(typeof(T), system); }

        public T GetSystem<T>() where T : class, IEntitySystem
        {
            if (!_entitySystems.TryGetValue(typeof(T), out var system))
            {
                return null;
            }

            if (system is T ins)
            {
                return ins;
            }

            return null;
        }
    }
}