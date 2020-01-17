using System;
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
}
