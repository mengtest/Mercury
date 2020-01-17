using System;
using Unity.Mathematics;

public static class DamageUtility
{
	public static float DealDmgFormula(BasicCapability data, float coefficient, DamageType damageType)
	{
		switch (damageType)
		{
			case DamageType.Physics:
				return data.phyAttack * coefficient;
			case DamageType.Magic:
				return data.magAttack * coefficient;
			default:
				return 0;
		}
	}

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
					return damage - (cod / 1f + cod);
				}
			case DamageType.True:
				return damage;
			default:
				throw new ArgumentException("你怎么回事小老弟?");
		}
	}
}
