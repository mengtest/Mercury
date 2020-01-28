/// <summary>
/// 可攻击和被攻击的实体
/// </summary>
public interface IAttackable
{
    float PhysicsAttack { get; }
    float MagicAttack { get; }

    /// <summary>
    /// 伤害计算链
    /// </summary>
    DamageChainCalculator DamageCalculator { get; }

    /// <summary>
    /// 攻击时造成的伤害
    /// </summary>
    /// <param name="coe">系数</param>
    /// <param name="damage">类型</param>
    /// <returns>伤害值</returns>
    Damage DealDamage(float coe, DamageType damage);

    /// <summary>
    /// 当被攻击时调用
    /// </summary>
    /// <param name="damage">伤害数据</param>
    void UnderAttack(in Damage damage);
}