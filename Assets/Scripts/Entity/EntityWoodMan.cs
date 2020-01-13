using UnityEngine;

public class EntityWoodMan : Entity, IAttackable
{
	protected override void Start()
	{
		base.Start();
		_healthPoint = 100_000;
		_maxHealthPoint = 100_000;
		_hpRecoverPerSec = 100;
	}

	public float DealDamage()
	{
		return 0;
	}

	public void UnderAttack(IAttackable attacker, float extra)
	{
		var damage = attacker.DealDamage() + extra;
		_healthPoint -= damage;
	}
}
