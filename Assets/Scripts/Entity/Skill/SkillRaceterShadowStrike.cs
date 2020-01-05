using UnityEngine;

public class SkillRaceterShadowStrike : AbstractSkill
{
	private float _duration;
	private readonly float _rawDura;
	//private float _damage;

	public SkillRaceterShadowStrike(ISkillable holder, float animTime) : base(holder)
	{
		_rawDura = animTime;
		_duration = animTime;
	}

	public override bool CanEnter(IFSMState current)
	{
		return current.GetType() == typeof(NormalState);
	}

	public override void OnAct()
	{
		_duration -= Time.deltaTime * 1000;
		if (_duration <= 0)
		{
			_skillHolder.UseSkill(typeof(StiffnessState));
			var sti = _skillHolder.FSMSystem.CurrentState as StiffnessState;
			sti.Duration = 200;
		}
	}

	public override void OnEnter()
	{
		var e = _skillHolder as EntityPlayer;
		var rigid = e.GetComponent<Rigidbody2D>();
		//var sr = e.GetComponent<SpriteRenderer>();
		//var move = e.GetProperty<MoveCapability>();
		//var face = sr.flipX ? new Vector2(1, 0) : new Vector2(-1, 0);
		//rigid.velocity = face * new Vector2(move.maxSpeed * 0.45f, 0);
		rigid.Sleep();
		//切换动画
	}

	public override void OnLeave()
	{
		_duration = _rawDura;
		var e = _skillHolder as EntityPlayer;
		var rigid = e.GetComponent<Rigidbody2D>();
		rigid.WakeUp();
	}
}
