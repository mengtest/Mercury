namespace Mercury
{
    /// <summary>
    /// 伤害计算器
    /// </summary>
    public interface IDamageCompute
    {
        /// <summary>
        /// 最大生命值
        /// </summary>
        DataChange<float> MaxHealth { get; }

        /// <summary>
        /// 每秒生命回复
        /// </summary>
        DataChange<float> HealthRecover { get; }

        /// <summary>
        /// 暴击伤害系数
        /// </summary>
        DataChange<float> CritCoe { get; }

        /// <summary>
        /// 暴击率
        /// </summary>
        DataChange<float> CritPr { get; }

        /// <summary>
        /// 攻击力
        /// </summary>
        /// <param name="type">类型</param>
        DataChange<float> GetAttack(DamageType type);

        /// <summary>
        /// 伤害增益
        /// </summary>
        /// <param name="type">伤害类型</param>
        float GetDamageIncome(DamageType type);

        /// <summary>
        /// 获取原始伤害
        /// </summary>
        /// <param name="coe">伤害系数</param>
        /// <param name="type">伤害类型</param>
        float GetDamage(float coe, DamageType type);

        /// <summary>
        /// 获取增益后额外伤害
        /// </summary>
        /// <param name="rawDamage">伤害系数</param>
        /// <param name="type">伤害类型</param>
        float GetExtraDamage(float rawDamage, DamageType type);

        /// <summary>
        /// 获取暴击的额外伤害
        /// </summary>
        /// <param name="damage">未暴击时伤害</param>
        float GetCritDamage(float damage);
    }
}