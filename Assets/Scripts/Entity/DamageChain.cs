using System;
using System.Collections.Generic;

public class DamageChain
{
    /// <summary>
    /// 0:物理
    /// 1:魔法
    /// </summary>
    private readonly List<DisorderList<DamageCalculator>> _subjoin = new List<DisorderList<DamageCalculator>>
    {
        new DisorderList<DamageCalculator>(DamageCalculator.defaultCalculator),
        new DisorderList<DamageCalculator>(DamageCalculator.defaultCalculator)
    };

    /// <summary>
    /// 0:物理
    /// 1:魔法
    /// 2:全伤害
    /// </summary>
    private readonly List<DisorderList<DamageCalculator>> _upgrade = new List<DisorderList<DamageCalculator>>
    {
        new DisorderList<DamageCalculator>(DamageCalculator.defaultCalculator),
        new DisorderList<DamageCalculator>(DamageCalculator.defaultCalculator),
        new DisorderList<DamageCalculator>(DamageCalculator.defaultCalculator)
    };

    private readonly IAttackable _attackable;
    private readonly float[] _damageSubjoin = {0f, 0f};
    private readonly float[] _damageUpgrade = {0f, 0f, 0f};

    public DamageChain(IAttackable attackable) { _attackable = attackable; }

    public void Add(in DamageCalculator calculator)
    {
        switch (calculator.incomeType)
        {
            case DamageIncome.Default:
                throw new ArgumentException();
            case DamageIncome.Subjoin when calculator.damageType == DamageType.True:
                throw new ArgumentException();
            case DamageIncome.Subjoin:
                _subjoin[(int) calculator.damageType].Add(calculator);
                RefuseSubjoinCache(calculator.damageType);
                RefuseUpgradeCache(calculator.damageType);
                return;
            case DamageIncome.Upgrade:
                _upgrade[(int) calculator.damageType].Add(calculator);
                RefuseSubjoinCache(calculator.damageType);
                RefuseUpgradeCache(calculator.damageType);
                return;
            default:
                throw new ArgumentException();
        }
    }

    public bool Remove(in DamageCalculator calculator)
    {
        switch (calculator.incomeType)
        {
            case DamageIncome.Default:
                throw new ArgumentException();
            case DamageIncome.Subjoin when calculator.damageType == DamageType.True:
                throw new ArgumentException();
            case DamageIncome.Subjoin:
                var res1 = _subjoin[(int) calculator.damageType].Remove(calculator);
                RefuseSubjoinCache(calculator.damageType);
                RefuseUpgradeCache(calculator.damageType);
                return res1;
            case DamageIncome.Upgrade:
                var res2 = _upgrade[(int) calculator.damageType].Remove(calculator);
                RefuseSubjoinCache(calculator.damageType);
                RefuseUpgradeCache(calculator.damageType);
                return res2;
            default:
                throw new ArgumentException();
        }
    }

    public float Calculate(float coe, DamageType damageType)
    {
        switch (damageType)
        {
            case DamageType.Physics:
                return DamageUtility.CalculateDamage(_attackable.PhysicsAttack,
                    coe,
                    _damageSubjoin[0],
                    _damageUpgrade[0]);
            case DamageType.Magic:
                return DamageUtility.CalculateDamage(_attackable.MagicAttack,
                    coe,
                    _damageSubjoin[1],
                    _damageUpgrade[1]);
            case DamageType.True:
                return coe + coe * _damageUpgrade[2];
            default:
                throw new ArgumentOutOfRangeException(nameof(damageType), damageType, "你怎么做到的？");
        }
    }

    private void RefuseSubjoinCache(DamageType type)
    {
        switch (type)
        {
            case DamageType.Physics:
                _damageSubjoin[0] = DamageUtility.CalculateAttackIncome(_attackable.PhysicsAttack, _subjoin[0]);
                return;
            case DamageType.Magic:
                _damageSubjoin[1] = DamageUtility.CalculateAttackIncome(_attackable.MagicAttack, _subjoin[1]);
                return;
            default:
                throw new ArgumentException();
        }
    }

    private void RefuseUpgradeCache(DamageType type) //TODO:全伤害加成没有写好
    {
        switch (type)
        {
            case DamageType.Physics:
                _damageUpgrade[0] = DamageUtility.CalculateAttackCoe(_upgrade[0]);
                return;
            case DamageType.Magic:
                _damageUpgrade[1] = DamageUtility.CalculateAttackCoe(_upgrade[1]);
                return;
            case DamageType.True:
                _damageUpgrade[2] = DamageUtility.CalculateAttackCoe(_upgrade[2]);
                return;
            default:
                throw new ArgumentException();
        }
    }
}