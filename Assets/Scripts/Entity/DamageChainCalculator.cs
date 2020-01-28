using System;
using System.Collections.Generic;

/// <summary>
/// 伤害链计算
/// </summary>
public class DamageChainCalculator
{
    /// <summary>
    /// 0:物理
    /// 1:魔法
    /// </summary>
    private readonly IReadOnlyList<DisorderList<DamageChain>> _subjoin = new List<DisorderList<DamageChain>>
    {
        new DisorderList<DamageChain>(DamageChain.defaultChain),
        new DisorderList<DamageChain>(DamageChain.defaultChain)
    };

    /// <summary>
    /// 0:物理
    /// 1:魔法
    /// 2:全伤害
    /// </summary>
    private readonly IReadOnlyList<DisorderList<DamageChain>> _upgrade = new List<DisorderList<DamageChain>>
    {
        new DisorderList<DamageChain>(DamageChain.defaultChain),
        new DisorderList<DamageChain>(DamageChain.defaultChain),
        new DisorderList<DamageChain>(DamageChain.defaultChain)
    };

    private readonly DisorderList<DamageCritChain> _crit = new DisorderList<DamageCritChain>();

    private readonly IAttackable _attackable;

    /// <summary>
    /// 攻击收益缓存
    /// 0:物理
    /// 1:魔法
    /// </summary>
    private readonly float[] _damageSubjoin = {0f, 0f};

    /// <summary>
    /// 伤害收益缓存
    /// 0:物理
    /// 1:魔法
    /// 2:全伤害
    /// </summary>
    private readonly float[] _damageUpgrade = {1f, 1f, 1f};

    private float _critCoe = 1.5f;

    public float CritProbability => DamageUtility.CalculateCritProbability(_attackable.Crit);

    public DamageChainCalculator(IAttackable attackable) { _attackable = attackable; }

    /// <summary>
    /// 添加收益链
    /// </summary>
    public void AddDamageChain(in DamageChain chain)
    {
        switch (chain.incomeType)
        {
            case DamageIncome.Default:
                throw new ArgumentException();
            case DamageIncome.Subjoin when chain.damageType == DamageType.True:
                throw new ArgumentException();
            case DamageIncome.Subjoin:
                _subjoin[(int) chain.damageType].Add(chain);
                RefuseSubjoinCache(chain.damageType);
                return;
            case DamageIncome.Upgrade:
                _upgrade[(int) chain.damageType].Add(chain);
                RefuseUpgradeCache(chain.damageType);
                return;
            default:
                throw new ArgumentException();
        }
    }

    /// <summary>
    /// 删除收益链
    /// </summary>
    public bool RemoveDamageChain(in DamageChain chain)
    {
        switch (chain.incomeType)
        {
            case DamageIncome.Default:
                throw new ArgumentException();
            case DamageIncome.Subjoin when chain.damageType == DamageType.True:
                throw new ArgumentException();
            case DamageIncome.Subjoin:
                var res1 = _subjoin[(int) chain.damageType].Remove(chain);
                RefuseSubjoinCache(chain.damageType);
                return res1;
            case DamageIncome.Upgrade:
                var res2 = _upgrade[(int) chain.damageType].Remove(chain);
                RefuseUpgradeCache(chain.damageType);
                return res2;
            default:
                throw new ArgumentException();
        }
    }

    public void AddCritChain(in DamageCritChain critChain)
    {
        _crit.Add(critChain);
        RefuseCrit();
    }

    public bool RemoveCritChain(in DamageCritChain critChain)
    {
        var res = _crit.Remove(critChain);
        RefuseCrit();
        return res;
    }

    private void RefuseCrit() { _critCoe = DamageUtility.CalculateCritIncome(_crit); }

    /// <summary>
    /// 计算技能伤害最终值
    /// </summary>
    /// <param name="coe">技能伤害系数</param>
    /// <param name="damageType">伤害类型</param>
    public float CalculateDamage(float coe, DamageType damageType)
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
                return coe * _damageUpgrade[2];
            default:
                throw new ArgumentOutOfRangeException(nameof(damageType), damageType, "你怎么做到的？");
        }
    }

    public float CalculateFinalDamage(float coe, DamageType damageType, out float extraCritDamage)
    {
        var dmg = CalculateDamage(coe, damageType);
        if (damageType == DamageType.True)
        {
            extraCritDamage = 0;
            return dmg;
        }

        var p = GameManager.Instance.Rand.NextFloat();
        if (p < CritProbability)
        {
            var res = dmg * _critCoe;
            extraCritDamage = res - dmg;
            return res;
        }

        extraCritDamage = 0;
        return dmg;
    }

    /// <summary>
    /// 刷新攻击力收益
    /// </summary>
    /// <param name="type">伤害类型</param>
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

    /// <summary>
    /// 刷新伤害收益。需要注意的是，DamageType.True在这里表示全伤害
    /// </summary>
    /// <param name="type">收益类型</param>
    private void RefuseUpgradeCache(DamageType type)
    {
        switch (type)
        {
            case DamageType.Physics:
                _damageUpgrade[0] = DamageUtility.CalculateAttackCoeAll(_upgrade[0], _upgrade[2]);
                return;
            case DamageType.Magic:
                _damageUpgrade[1] = DamageUtility.CalculateAttackCoeAll(_upgrade[1], _upgrade[2]);
                return;
            case DamageType.True:
                _damageUpgrade[0] = DamageUtility.CalculateAttackCoeAll(_upgrade[0], _upgrade[2]);
                _damageUpgrade[1] = DamageUtility.CalculateAttackCoeAll(_upgrade[1], _upgrade[2]);
                _damageUpgrade[2] = DamageUtility.CalculateAttackCoe(_upgrade[2]);
                return;
            default:
                throw new ArgumentException();
        }
    }
}