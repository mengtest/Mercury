namespace Mercury
{
    public interface IAttackable
    {
        DamageData DamageRawData { get; }

        /// <summary>
        /// 伤害系统组件
        /// </summary>
        IDamageCompute DamageSystem { get; }

        /// <summary>
        /// 计算伤害
        /// </summary>
        /// <param name="coe">伤害系数</param>
        /// <param name="type">伤害类型</param>
        /// <returns>计算结果</returns>
        Damage CalculateDamage(float coe, DamageType type);

        /// <summary>
        /// 攻击
        /// </summary>
        /// <param name="damage">将要造成的伤害</param>
        /// <param name="target">目标</param>
        /// <returns>最终打出的伤害</returns>>
        Damage DealDamage(in Damage damage, IAttackable target);

        /// <summary>
        /// 当被攻击时调用
        /// </summary>
        /// <param name="damage">伤害数据</param>
        void UnderAttack(in Damage damage);
    }
}