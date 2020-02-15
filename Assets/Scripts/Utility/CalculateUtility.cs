using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.Mathematics;

public static class CalculateUtility
{
    public static readonly Func<DisorderList<float>, float> FormulaSum = chain => chain.Sum();

    public static readonly Func<DisorderList<float>, float> FormulaMultiply =
        chain => chain.Aggregate(1f, (current, variation) => current * variation);

    /// <summary>
    /// 减伤公式
    /// </summary>
    /// <param name="damage">本应该造成的伤害</param>
    /// <param name="attackedData">被攻击者数据</param>
    /// <param name="damageType">伤害类型</param>
    /// <returns>承受伤害</returns>
    public static float ReduceDmgFormula(float damage, BasicCapability attackedData, DamageType damageType)
    {
        switch (damageType)
        {
            case DamageType.Physics:
            {
                var res = damage - attackedData.phyDefense;
                return math.max(damage * 0.01f, res);
            }
            case DamageType.Magic:
            {
                var cod = attackedData.magDefense * 0.001f;
                return damage - damage * (cod / 1f + cod);
            }
            case DamageType.True:
                return damage;
            default:
                throw new ArgumentException("你怎么回事小老弟?");
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float CalculateCritProbability(float value)
    {
        if (math.abs(value) < 0.000001f)
        {
            return 0;
        }

        var a = value * 0.003f;
        return a / (a + 1);
    }

    public static void AddChain(CalculationChain chain, float income)
    {
        chain.Add(income);
        chain.RefreshResult();
    }

    public static bool RemoveChain(CalculationChain chain, float income)
    {
        var res = chain.Remove(income);
        chain.RefreshResult();
        return res;
    }
}