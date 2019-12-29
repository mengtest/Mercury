public class EntityPlayer : Entity, IAttackable
{
	private readonly BasicCapability _basicCapability;
	private readonly ElementAffinity _elementAffinity;

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
		finalDamage *= _basicCapability.CriticalPoint;
		return finalDamage;
	}

	public void UnderAttack(IAttackable attacker)
	{
		var damage = attacker.DealDamage();
		_healthPoint -= damage;
	}
}
