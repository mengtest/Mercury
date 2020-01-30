using System;

/// <summary>
/// 可攻击和被攻击的实体
/// </summary>
public interface IAttackable
{
    /// <summary>
    /// 物理攻击力
    /// </summary>
    float PhysicsAttack { get; }

    /// <summary>
    /// 魔法攻击力
    /// </summary>
    float MagicAttack { get; }

    /// <summary>
    /// 暴击系数
    /// </summary>
    int Crit { get; }

    /// <summary>
    /// 伤害计算链
    /// </summary>
    DamageChainCalculator DamageCalculator { get; }

    /// <summary>
    /// 攻击目标时触发，arg1表示攻击伤害，arg2表示目标
    /// </summary>
    event Action<Damage, IAttackable> OnAttackTarget;

    /// <summary>
    /// 计算伤害
    /// </summary>
    /// <param name="coe">系数</param>
    /// <param name="damage">类型</param>
    Damage CalculateDamage(float coe, DamageType damage);

    /// <summary>
    /// 攻击
    /// </summary>
    /// <param name="damage">将要造成的伤害伤害</param>
    /// <param name="target">目标</param>
    Damage DealDamage(in Damage damage, IAttackable target);

    /// <summary>
    /// 当被攻击时调用
    /// </summary>
    /// <param name="damage">伤害数据</param>
    void UnderAttack(in Damage damage);
}