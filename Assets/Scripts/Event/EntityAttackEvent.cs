using System;

namespace Mercury
{
    public abstract class EntityAttackEvent : EventArgs
    {
        public readonly IAttackable source;
        public readonly IAttackable target;
        public readonly Damage damage;

        public Damage Result { get; set; }

        public EntityAttackEvent(IAttackable source, IAttackable target, Damage damage)
        {
            this.source = source;
            this.target = target;
            this.damage = damage;
            Result = damage;
        }

        public class Deal : EntityAttackEvent
        {
            public Deal(IAttackable source, IAttackable target, Damage damage) : base(source, target, damage) { }
        }

        public class UnderAttack : EntityAttackEvent
        {
            public UnderAttack(IAttackable source, IAttackable target, Damage damage) : base(source, target, damage) { }
        }
    }
}