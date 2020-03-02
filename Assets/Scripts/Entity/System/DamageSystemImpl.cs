using System;
using UnityEngine;

namespace Mercury
{
    public interface IDamageSystem : IEntitySystem
    {
        /// <summary>
        /// 当攻击时触发
        /// </summary>
        event EventHandler<EntityAttackEvent.Attack> OnAttack;

        /// <summary>
        /// 当被攻击时触发
        /// </summary>
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
        Damage Attack(Damage damage, IAttackable target);

        /// <summary>
        /// 当被攻击时调用
        /// </summary>
        /// <param name="damage">伤害数据</param>
        void UnderAttack(Damage damage);
    }

    public class DamageSystemImpl : MonoBehaviour, IDamageSystem
    {
        private IDamageCompute _compute;
        private IAttackable _owner;

        public event EventHandler<EntityAttackEvent.Attack> OnAttack;
        public event EventHandler<EntityAttackEvent.UnderAttack> OnUnderAttack;

        public DamageSystemImpl Init(IAttackable owner)
        {
            _owner = owner;
            _compute = _owner.DamageCompute;
            _owner.Health = Misc.AddDataChange(_compute.MaxHealth);
            return this;
        }

        public Damage CalculateDamage(float coe, DamageType type)
        {
            var pr = Misc.AddDataChange(_compute.CritPr);
            var dmg = _compute.GetDamage(coe, type);
            var ex = _compute.GetExtraDamageIncome(dmg, type);
            var realDmg = dmg + ex;
            var rand = new Unity.Mathematics.Random((uint) DateTime.Now.Ticks);
            var critDmg = 0f;
            if (rand.NextFloat() < pr)
            {
                critDmg = _compute.GetExtraCritDamage(realDmg);
            }

            return new Damage(_owner, realDmg, critDmg, type);
        }

        public Damage Attack(Damage damage, IAttackable target)
        {
            if (OnAttack == null)
            {
                return damage;
            }

            var atkEvent = new EntityAttackEvent.Attack(_owner, target, damage);
            OnAttack(this, atkEvent);
            return atkEvent.Result;
        }

        public void UnderAttack(Damage damage)
        {
            float result;
            if (OnUnderAttack == null)
            {
                result = damage.FinalDamage;
            }
            else
            {
                var e = new EntityAttackEvent.UnderAttack(damage.source, _owner, damage);
                OnUnderAttack(this, e);
                result = e.Result.FinalDamage;
            }

            _owner.Health -= result;
            //TODO:死亡应该是个函数,且发布死亡事件
        }

        private void Update()
        {
            var nowHealth = _owner.Health;
            var maxHealth = Misc.AddDataChange(_compute.MaxHealth);
            var tryHeal = nowHealth + Misc.DataChangePreTick(nowHealth, Misc.AddDataChange(_compute.HealthRecover));
            _owner.Health = tryHeal > maxHealth ? maxHealth : tryHeal;
        }
    }
}