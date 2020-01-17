public struct Damage
{
	public readonly float value;
	public readonly DamageType type;
	public readonly IAttackable source;

	public Damage(IAttackable source, float value, DamageType type)
	{
		this.value = value;
		this.type = type;
		this.source = source;
	}
}
