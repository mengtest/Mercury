/// <summary>
/// 可攻击和被攻击的实体
/// </summary>
public interface IAttackable
{
	/// <summary>
	/// 产生的伤害
	/// </summary>
	/// <returns>伤害值</returns>
	float DealDamage();

	/// <summary>
	/// 当被攻击时调用
	/// </summary>
	/// <param name="attacker">攻击者</param>
	void UnderAttack(IAttackable attacker);
}
