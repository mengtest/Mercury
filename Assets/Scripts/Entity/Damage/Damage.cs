namespace Mercury
{
    /// <summary>
    /// 伤害数据
    /// </summary>
    public struct Damage
    {
        /// <summary>
        /// 伤害类型
        /// </summary>
        public readonly DamageType type;

        /// <summary>
        /// 伤害来源
        /// </summary>
        public readonly IAttackable source;

        /// <summary>
        /// 伤害量
        /// </summary>
        public float Value { get; set; }

        /// <summary>
        /// 暴击伤害量，0则本次攻击未暴击
        /// </summary>
        public float ExtraCritValue { get; set; }

        /// <summary>
        /// 最终伤害量，等于伤害量+暴击伤害量
        /// </summary>
        public float FinalDamage => Value + ExtraCritValue;

        public Damage(IAttackable source, float value, float extraCritValue, DamageType type)
        {
            Value = value;
            this.type = type;
            this.source = source;
            ExtraCritValue = extraCritValue;
        }
    }
}