using UnityEngine;

namespace Mercury
{
    /// <summary>
    /// 抽象玩家
    /// TODO:所有需要的元素都在注册时装配，不再被继承
    /// </summary>
    public abstract class EntityPlayer : Entity, IUpdatable, IAttackable, IMovable
    {
        protected EntityPlayer(AssetLocation id, DamageData damageData, MotionData motionData) : base(id)
        {
            DamageRawData = damageData;
            MotionRawData = motionData;
            AddComponent(DamageRawData);
            AddComponent(MotionRawData);
        }

        public virtual void OnUpdate() { }

        public DamageData DamageRawData { get; }
        public abstract IDamageCompute DamageSystem { get; }

        public abstract Damage CalculateDamage(float coe, DamageType type);

        public Damage DealDamage(in Damage damage, IAttackable target)
        {
            GameManager.Instance.EventBus.Publish(this, new EntityAttackEvent(this, target, damage));
            return damage;
        }

        public abstract void UnderAttack(in Damage damage);

        public MotionData MotionRawData { get; }
        public abstract IMotionCompute MoveSystem { get; }
        public abstract Vector2 Velocity { get; }

        public abstract void Move(Vector2 distance);
    }
}