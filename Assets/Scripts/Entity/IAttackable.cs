/// <summary>
/// 可攻击和被攻击的实体
/// </summary>
public interface IAttackable
{
	/// <summary>
	/// 攻击时造成的伤害
	/// </summary>
	/// <returns>伤害值</returns>
	Damage DealDamage(float coefficient, DamageType damage);

	/// <summary>
	/// 当被攻击时调用
	/// </summary>
	void UnderAttack(in Damage damage);
}
