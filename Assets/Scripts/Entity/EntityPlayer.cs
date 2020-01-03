using System;
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

	#region IBuffable
	public void AddBuff(IBuff buff)
	{
		_buffs.AddBuff(buff);
	}

	public IBuff GetBuff(Type buffType, BuffVariant variant)
	{
		return _buffs.GetBuff(buffType, variant);
	}

	public bool RemoveBuff(Type buffType, BuffVariant variant)
	{
		return _buffs.RemoveBuff(buffType, variant);
	}

	public bool TryGetBuff(Type buffType, BuffVariant variant, out IBuff buff)
	{
		return _buffs.TryGetBuff(buffType, variant, out buff);
	}

	public bool HasBuff(Type buffType, BuffVariant variant)
	{
		return _buffs.HasBuff(buffType, variant);
	}
	#endregion

	#region IAttackable
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
	#endregion
}
