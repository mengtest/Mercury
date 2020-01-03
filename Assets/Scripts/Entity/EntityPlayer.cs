using UnityEngine;

/// <summary>
/// 玩家
/// </summary>
public class EntityPlayer : Entity, IAttackable, IBuffable
{
	[SerializeField]
	private BasicCapability _basicCapability;
	[SerializeField]
	private ElementAffinity _elementAffinity;

	private BuffWapper _buffs;

	protected override void Start()
	{
		base.Start();
		_basicCapability = new BasicCapability();
		_elementAffinity = new ElementAffinity();
		SetProperty(_basicCapability);
		SetProperty(_elementAffinity);
		_buffs = new BuffWapper(this);
	}

	protected override void Update()
	{
		base.Update();
		_buffs.OnUpdate();
	}

	public void AddBuff(IBuff buff)
	{
		_buffs.AddBuff(buff);
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
