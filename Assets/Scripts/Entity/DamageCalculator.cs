using System;

/// <summary>
/// 伤害公式链元素
/// </summary>
public struct DamageCalculator : IEquatable<DamageCalculator>
{
    public static DamageCalculator defaultCalculator =
        new DamageCalculator(DamageIncome.Default, null, 0, DamageType.Physics);

    /// <summary>
    /// 收益类型
    /// </summary>
    public readonly DamageIncome incomeType;

    /// <summary>
    /// 伤害类型
    /// </summary>
    public readonly DamageType damageType;

    /// <summary>
    /// 收益公式
    /// </summary>
    public readonly float coefficient;

    /// <summary>
    /// 伤害来源
    /// </summary>
    public readonly object source;

    public DamageCalculator(
        DamageIncome incomeType,
        object source,
        float coefficient,
        DamageType damageType)
    {
        this.incomeType = incomeType;
        this.coefficient = coefficient;
        this.damageType = damageType;
        this.source = source;
    }

    public bool Equals(DamageCalculator other)
    {
        return other.coefficient == coefficient &&
               other.incomeType == incomeType &&
               other.source == source &&
               other.damageType == damageType;
    }
}