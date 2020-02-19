using System;
using System.Runtime.CompilerServices;

public class DamageCalculator
{
    private readonly IAttackable _attackable;

    /// <summary>
    /// 0:物理
    /// 1:魔法
    /// </summary>
    private readonly CalculationChain[] _atk = new CalculationChain[2];

    /// <summary>
    /// 0:物理
    /// 1:魔法
    /// 2:全伤
    /// </summary>
    private readonly CalculationChain[] _dmg = new CalculationChain[3];

    /// <summary>
    /// 暴击系数
    /// </summary>
    private readonly CalculationChain _critCoe = new CalculationChain(CalculateUtility.FormulaMultiply);

    /// <summary>
    /// 0:暴击值
    /// 1:暴击率
    /// </summary>
    private readonly CalculationChain[] _critPr = new CalculationChain[2];

    /// <summary>
    /// 物理攻击力
    /// </summary>
    public DataChange PhysicsAttack
    {
        get
        {
            var phyAtk = _attackable.PhysicsAttack;
            return new DataChange(phyAtk, _atk[0].Result * phyAtk);
        }
    }

    /// <summary>
    /// 魔法攻击力
    /// </summary>
    public DataChange MagicAttack
    {
        get
        {
            var magAtk = _attackable.MagicAttack;
            return new DataChange(magAtk, _atk[0].Result * magAtk);
        }
    }

    /// <summary>
    /// 物理增伤
    /// </summary>
    public float PhysicsDamageIncome => _dmg[0].Result;

    /// <summary>
    /// 魔法增伤
    /// </summary>
    public float MagicDamageIncome => _dmg[1].Result;

    /// <summary>
    /// 全伤增伤
    /// </summary>
    public float AllDamageIncome => _dmg[2].Result;

    /// <summary>
    /// 暴击倍数
    /// </summary>
    public DataChange CritCoe => new DataChange(_attackable.CritCoefficient, _critCoe.Result);

    /// <summary>
    /// 暴击率
    /// </summary>
    public DataChange CritPr =>
        new DataChange(CalculateUtility.CalculateCritProbability(_attackable.CritProbability), CalculateUtility.CalculateCritProbability(_critPr[0].Result) + _critPr[1].Result);

    public DamageCalculator(IAttackable attackable)
    {
        _attackable = attackable;
        _atk[0] = new CalculationChain(CalculateUtility.FormulaSum);
        _atk[1] = new CalculationChain(CalculateUtility.FormulaSum);
        _dmg[0] = new CalculationChain(CalculateUtility.FormulaMultiply);
        _dmg[1] = new CalculationChain(CalculateUtility.FormulaMultiply);
        _dmg[2] = new CalculationChain(CalculateUtility.FormulaMultiply);
        _critPr[0] = new CalculationChain(CalculateUtility.FormulaSum);
        _critPr[1] = new CalculationChain(CalculateUtility.FormulaSum);
    }

    public void AddAttack(float income, DamageType type)
    {
        if (type == DamageType.True)
        {
            throw new InvalidOperationException();
        }

        CalculateUtility.AddChain(_atk[(int) type], income);
    }

    public void RemoveAttack(float income, DamageType type)
    {
        if (type == DamageType.True)
        {
            throw new InvalidOperationException();
        }

        CalculateUtility.RemoveChain(_atk[(int) type], income);
    }

    public void AddDamage(float income, DamageType type) { CalculateUtility.AddChain(_dmg[(int) type], income); }

    public void RemoveDamage(float income, DamageType type) { CalculateUtility.RemoveChain(_dmg[(int) type], income); }

    public void AddCritCoe(float coe) { CalculateUtility.AddChain(_critCoe, coe); }

    public void RemoveCoe(float coe) { CalculateUtility.RemoveChain(_critCoe, coe); }

    public void AddCritPr(float pr, CritPrType type) { CalculateUtility.AddChain(_critPr[(int) type], pr); }

    public void RemoveCritPr(float pr, CritPrType type) { CalculateUtility.RemoveChain(_critPr[(int) type], pr); }

    /// <summary>
    /// 基础伤害计算
    /// </summary>
    /// <param name="skillCoe">技能系数</param>
    /// <param name="type">伤害类型</param>
    public Damage SimpleDamage(float skillCoe, DamageType type)
    {
        switch (type)
        {
            case DamageType.Physics:
            {
                var dmg = DefaultAtkDmg(skillCoe, PhysicsAttack.Add, PhysicsDamageIncome, AllDamageIncome);
                var crit = GetCritDamage(dmg, CritCoe.Multiply, CritPr.Add);
                return new Damage(_attackable, dmg, crit, type);
            }
            case DamageType.Magic:
            {
                var dmg = DefaultAtkDmg(skillCoe, MagicAttack.Add, MagicDamageIncome, AllDamageIncome);
                var crit = GetCritDamage(dmg, CritCoe.Multiply, CritPr.Add);
                return new Damage(_attackable, dmg, crit, type);
            }
            case DamageType.True:
                return new Damage(_attackable, skillCoe * AllDamageIncome, 0, type);
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    /// <summary>
    /// 技能系数*攻击力（除真伤外）*伤害增益
    /// </summary>
    public float GetDamage(float skillCoe, DamageType type)
    {
        //Debug.Log($"coe{skillCoe} atk{PhysicsAttack.Add} phy:{PhysicsDamageIncome} all:{AllDamageIncome}");
        switch (type)
        {
            case DamageType.Physics:
                return DefaultAtkDmg(skillCoe, PhysicsAttack.Add, PhysicsDamageIncome, AllDamageIncome);
            case DamageType.Magic:
                return DefaultAtkDmg(skillCoe, MagicAttack.Add, MagicDamageIncome, AllDamageIncome);
            case DamageType.True:
                return skillCoe * AllDamageIncome;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    /// <summary>
    /// 暴击伤害
    /// </summary>
    /// <param name="dmg">伤害</param>
    /// <param name="coe">暴击系数</param>
    /// <param name="pr">暴击率</param>
    /// <returns>额外伤害</returns>
    public static float GetCritDamage(float dmg, float coe, float pr)
    {
        var rand = new Unity.Mathematics.Random((uint) DateTime.Now.Ticks).NextFloat();
        if (rand < pr)
        {
            var res = dmg * coe;
            return res - dmg;
        }

        return 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private float DefaultAtkDmg(float coe, float atk, float dmg1, float dmg2)
    {
        //var res = coe * 0.01f * atk * dmg1 * dmg2;
        //Debug.Log($"coe{coe} atk{atk} phy{dmg1} all{dmg2} res{res} crit:{CritCoe.raw}|{CritCoe.change}");
        return coe * 0.01f * atk * dmg1 * dmg2;
    }
}