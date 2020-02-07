using System;
using Unity.Mathematics;

/// <summary>
/// 伤害公式链元素
/// </summary>
public struct DamageChain : IEquatable<DamageChain>
{
    public static DamageChain defaultChain =
        new DamageChain(DamageIncome.Default, null, 0, DamageType.Physics);

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

    /// <param name="incomeType">该计算者类型</param>
    /// <param name="source">伤害源</param>
    /// <param name="coefficient">系数</param>
    /// <param name="damageType">伤害类型。当incomeType为Subjoin时，该值填True无意义。当incomeType为Upgrade时，该值填True表示全伤害增益</param>
    public DamageChain(
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

    public bool Equals(DamageChain other)
    {
        return math.abs(other.coefficient - coefficient) < 0.00001f &&
               other.incomeType == incomeType &&
               other.source == source &&
               other.damageType == damageType;
    }
}