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
            /// <summary>
            /// 最终造成的伤害，可被委托修改
            /// </summary>
            public Damage Result { get; set; }

            public Attack(IAttackable source, IAttackable target, Damage damage) : base(source, target, damage) { Result = damage; }
        }

        public class UnderAttack : EntityAttackEvent
        {
            /// <summary>
            /// 最终造成的伤害，可被委托修改
            /// </summary>
            public Damage Result { get; set; }

            public UnderAttack(IAttackable source, IAttackable target, Damage damage) : base(source, target, damage) { Result = damage; }
        }
    }
}