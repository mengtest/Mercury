using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.Mathematics;

public static class DamageUtility
{
    /// <summary>
    /// 造成伤害公式
    /// </summary>
    /// <param name="data">攻击者数据</param>
    /// <param name="coe">倍数</param>
    /// <param name="damageType">伤害类型</param>
    /// <returns>造成伤害</returns>
    public static float DealDmgFormula(BasicCapability data, float coe, DamageType damageType)
    {
        switch (damageType)
        {
            case DamageType.Physics:
                return data.phyAttack * coe;
            case DamageType.Magic:
                return data.magAttack * coe;
            case DamageType.True:
                return coe;
            default:
                throw new ArgumentException("?");
        }
    }

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

    /// <summary>
    /// 计算攻击力收益
    /// </summary>
    /// <param name="rawAttack">初始攻击力</param>
    /// <param name="incomeChain">攻击力收益链</param>
    public static float CalculateAttackIncome(float rawAttack, IList<DamageChain> incomeChain)
    {
        var income = 0f;
        foreach (var c in incomeChain)
        {
            income += rawAttack * c.coefficient;
        }

        return income; //基础攻击力+攻击力收益
    }

    /// <summary>
    /// 计算伤害收益
    /// </summary>
    /// <param name="coeChain">收益链</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float CalculateAttackCoe(IList<DamageChain> coeChain) =>
        coeChain.Aggregate(1f, (current, c) => current * c.coefficient);

    /// <summary>
    /// 计算全伤害收益
    /// </summary>
    /// <param name="coeChain">普通伤害收益链</param>
    /// <param name="allChain">全伤害收益链</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float CalculateAttackCoeAll(IList<DamageChain> coeChain, IList<DamageChain> allChain) =>
        allChain.Aggregate(CalculateAttackCoe(coeChain), (current, c) => current * c.coefficient);

    /// <summary>
    /// 计算最终伤害
    /// </summary>
    /// <param name="rawAttack">初始攻击力</param>
    /// <param name="coe">技能/伤害系数</param>
    /// <param name="incomeChain">攻击力收益链</param>
    /// <param name="coeChain">伤害收益链</param>
    public static float CalculateDamage(
        float rawAttack,
        float coe,
        IList<DamageChain> incomeChain,
        IList<DamageChain> coeChain)
    {
        var atk = rawAttack + CalculateAttackIncome(rawAttack, incomeChain);
        var dmg = coe * atk / 100;
        var atkCoe = CalculateAttackCoe(coeChain);
        return dmg * atkCoe;
    }

    /// <summary>
    /// 计算最终伤害
    /// </summary>
    /// <param name="rawAttack">初始攻击力</param>
    /// <param name="coe">技能/伤害系数</param>
    /// <param name="income">攻击力收益</param>
    /// <param name="dmgCoe">伤害收益</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float CalculateDamage(float rawAttack, float coe, float income, float dmgCoe) =>
        coe * (rawAttack + income) / 100 * dmgCoe;
}