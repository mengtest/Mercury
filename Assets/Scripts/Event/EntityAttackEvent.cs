using System;

namespace Mercury
{
    /// <summary>
    /// 实体攻击事件
    /// </summary>
    public abstract class EntityAttackEvent : EventArgs
    {
        /// <summary>
        /// 伤害来源，也就是攻击者
        /// </summary>
        public readonly IAttackable source;

        /// <summary>
        /// 目标，也就是被攻击者
        /// </summary>
        public readonly IAttackable target;

        /// <summary>
        /// 伤害来源造成的伤害
        /// </summary>
        public readonly Damage damage;

        public EntityAttackEvent(IAttackable source, IAttackable target, Damage damage)
        {
            this.source = source;
            this.target = target;
            this.damage = damage;
        }

        public class Attack : EntityAttackEvent
        {
            public Attack(IAttackable source, IAttackable target, Damage damage) : base(source, target, damage) { }
        }

        public class UnderAttack : EntityAttackEvent
        {
            public UnderAttack(IAttackable source, IAttackable target, Damage damage) : base(source, target, damage) { }
        }
    }
}