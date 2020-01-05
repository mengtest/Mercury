using System;
using UnityEngine;

/// <summary>
/// 玩家
/// </summary>
public class EntityPlayer : Entity, IAttackable, IBuffable
{
	[SerializeField]
	private BasicCapability _basicCapability = new BasicCapability();
	[SerializeField]
	private ElementAffinity _elementAffinity = new ElementAffinity();
	[SerializeField]
	private MoveCapability _moveCapability = new MoveCapability();
	[SerializeField]
	private BasicState _basicState = new BasicState();

	private BuffWapper _buffs;

	protected override void Start()
	{
		base.Start();
		_healthPoint = 1;
		_maxHealthPoint = 100;
		_hpRecoverPerSec = 0.5f;
		SetProperty(_basicCapability);
		SetProperty(_elementAffinity);
		SetProperty(_moveCapability);
		SetProperty(_basicState);
		AddSystem<MoveSystem>();
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

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag("Step"))
		{
			_basicState.isOnStep = true;
			_basicState.standedStep = collision;
		}
	}

	private void OnCollisionStay2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag("Step"))
		{
			_basicState.isOnStep = true;
			_basicState.standedStep = collision;
		}
	}

	private void OnCollisionExit2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag("Step"))
		{
			_basicState.isOnStep = false;
			_basicState.standedStep = null;
		}
	}
}
