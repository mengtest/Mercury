public struct Damage
{
    /// <summary>
    /// 伤害量
    /// </summary>
    public readonly float value;
    /// <summary>
    /// 暴击伤害量，0则本次攻击未暴击
    /// </summary>
    public readonly float extraCritValue;
    /// <summary>
    /// 伤害类型
    /// </summary>
    public readonly DamageType type;
    /// <summary>
    /// 伤害来源
    /// </summary>
    public readonly IAttackable source;

    /// <summary>
    /// 最终伤害量，等于伤害量+暴击伤害量
    /// </summary>
    public float FinalDamage => value + extraCritValue;

    public Damage(IAttackable source, float value, float extraCritValue, DamageType type)
    {
        this.value = value;
        this.type = type;
        this.source = source;
        this.extraCritValue = extraCritValue;
    }
}