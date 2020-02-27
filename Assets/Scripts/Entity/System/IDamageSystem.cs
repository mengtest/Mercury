using System;

namespace Mercury
{
    public interface IDamageSystem : IEntitySystem, IUpdatable
    {
        event EventHandler<EntityAttackEvent.Deal> OnDealDamage;

        event EventHandler<EntityAttackEvent.UnderAttack> OnUnderAttack;

        /// <summary>
        /// 计算伤害
        /// </summary>
        /// <param name="coe">伤害系数</param>
        /// <param name="type">伤害类型</param>
        /// <returns>计算结果</returns>
        Damage CalculateDamage(float coe, DamageType type);

        /// <summary>
        /// 攻击，应该发布EntityAttackEvent事件
        /// </summary>
        /// <param name="damage">将要造成的伤害</param>
        /// <param name="target">目标</param>
        /// <returns>最终打出的伤害</returns>
        Damage DealDamage(Damage damage, IAttackable target);

        /// <summary>
        /// 当被攻击时调用
        /// </summary>
        /// <param name="damage">伤害数据</param>
        void UnderAttack(Damage damage);
    }

    public class DamageSystemImpl : IDamageSystem
    {
        private readonly IDamageCompute _compute;
        private readonly IAttackable _owner;

        public event EventHandler<EntityAttackEvent.Deal> OnDealDamage;
        public event EventHandler<EntityAttackEvent.UnderAttack> OnUnderAttack;

        public DamageSystemImpl(IAttackable owner)
        {
            _owner = owner;
            _compute = _owner.DamageCompute;
            _owner.Health = Misc.AddData(_compute.MaxHealth);
        }

        public Damage CalculateDamage(float coe, DamageType type)
        {
            var pr = Misc.AddData(_compute.CritPr);
            var dmg = _compute.GetDamage(coe, type);
            var ex = _compute.GetExtraDamage(dmg, type);
            var realDmg = dmg + ex;
            var rand = new Unity.Mathematics.Random((uint) DateTime.Now.Ticks);
            var critDmg = 0f;
            if (rand.NextFloat() < pr)
            {
                critDmg = _compute.GetCritDamage(realDmg);
            }

            return new Damage(_owner, realDmg, critDmg, type);
        }

        public Damage DealDamage(Damage damage, IAttackable target)
        {
            var atkEvent = new EntityAttackEvent.Deal(_owner, target, damage);
            OnDealDamage?.Invoke(this, atkEvent);
            return atkEvent.Result;
        }

        public void UnderAttack(Damage damage)
        {
            var atkedEvent = new EntityAttackEvent.UnderAttack(damage.source, _owner, damage);
            OnUnderAttack?.Invoke(this, atkedEvent);
            _owner.Health -= atkedEvent.Result.FinalDamage;
            //TODO:死亡应该是个函数,且发布死亡事件
        }

        public void OnUpdate()
        {
            var nowHealth = _owner.Health;
            var maxHealth = Misc.AddData(_compute.MaxHealth);
            var tryHeal = nowHealth + Misc.DataChangePreTick(nowHealth, Misc.AddData(_compute.HealthRecover));
            _owner.Health = tryHeal > maxHealth ? maxHealth : tryHeal;
        }
    }
}