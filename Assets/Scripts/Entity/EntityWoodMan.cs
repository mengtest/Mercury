public class EntityWoodMan : Entity, IAttackable
{
	protected override void Start()
	{
		base.Start();
		_healthPoint = 100_000;
		_maxHealthPoint = 100_000;
		_hpRecoverPerSec = 100;
	}

	public Damage DealDamage(float coefficient, DamageType damageType)
	{
		return new Damage(this, 0, damageType);
	}

	public void UnderAttack(in Damage damage)
	{
		_healthPoint -= damage.value;
		UIManager.Instance.ShowDamage(transform, (int)damage.value, damage.type);
	}
}
