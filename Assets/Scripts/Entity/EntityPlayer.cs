using UnityEngine;

public class EntityPlayer : Entity, IAttackable, IBuffable
{
	[SerializeField]
	private BasicCapability _basicCapability;
	[SerializeField]
	private ElementAffinity _elementAffinity;

	public EntityPlayer()
	{
		_basicCapability = new BasicCapability();
		_elementAffinity = new ElementAffinity();
		SetProperty(_basicCapability);
		SetProperty(_elementAffinity);
	}

	public float DealDamage()
	{
		//例子
		var finalDamage = 1;
		finalDamage *= _basicCapability.criticalPoint;
		return finalDamage;
	}

	public void UnderAttack(IAttackable attacker)
	{
		var damage = attacker.DealDamage();
		_healthPoint -= damage;
	}
}
