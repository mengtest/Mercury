using System;

namespace Mercury
{
    public class EntityAttackEvent : EventArgs
    {
        public readonly IAttackable source;
        public readonly IAttackable target;
        public readonly Damage damage;

        public EntityAttackEvent(IAttackable source, IAttackable target, Damage damage)
        {
            this.source = source;
            this.target = target;
            this.damage = damage;
        }
    }
}