using System.Collections.Generic;
using UnityEngine;

public class SkillRaceterShadowStrike : AbstractSkill
{
	private float _duration;
	private readonly float _rawDura;
	private readonly Rigidbody2D _rigid;
	private readonly GameObject _se;
	private readonly Animator _seAnim;
	private readonly EntityPlayer _player;
	private float _g;
	private readonly SkillObject _seColl;
	private readonly HashSet<IAttackable> _attacked = new HashSet<IAttackable>();

	public SkillRaceterShadowStrike(ISkillable holder) : base(holder, 12)
	{
		_player = holder as EntityPlayer;
		_rigid = _player.GetComponent<Rigidbody2D>();
		_se = GameManager.Instance.GetEffect(Consts.PREFAB_SE_SkillRaceterShadowStrike);
		_seAnim = _se.GetComponent<Animator>();
		_seColl = _se.GetComponent<SkillObject>();
		_se.Hide();
		_rawDura = GetClipLength(_seAnim, Consts.PREFAB_SE_SkillRaceterShadowStrike);
		_duration = _rawDura;
	}

	~SkillRaceterShadowStrike()
	{
		GameManager.Instance.RecycleEffect(_se);
	}

	public override bool CanEnter(IFSMState current)
	{
		return current.GetType() == typeof(NormalState) && IsCoolDown();
	}

	public override void OnAct()
	{
		_duration -= Time.deltaTime;
		var playerVelocity = _rigid.velocity;
		_se.transform.position = _player.transform.position;
		_rigid.AddForce(new Vector2(-playerVelocity.x * (_rawDura - _duration / _rawDura) * 0.01f, 0));
		if (_seColl.Contact && _seColl.Contact.CompareTag("Entity"))
		{
			var e = _seColl.Contact.GetComponent<Entity>();
			if (e is IAttackable attackable)
			{
				if (!_attacked.Contains(attackable))
				{
					attackable.UnderAttack(_player.DealDamage(0.95f, DamageType.Physics));
					_attacked.Add(attackable);
				}
			}
		}
		if (_duration <= 0)
		{
			_se.Hide();
			_skillHolder.UseSkill(typeof(StiffnessState));
			var sti = _skillHolder.FSMSystem.CurrentState as StiffnessState;
			sti.Duration = 0;
		}
	}

	public override void OnEnter()
	{
		_se.Show();
		var seR = _se.transform.eulerAngles;
		var pR = _player.transform.eulerAngles;
		_se.transform.eulerAngles = new Vector3(seR.x, pR.y - 180, seR.z);
		_se.transform.position = _player.transform.position;
		_rigid.velocity = Vector2.zero;
		var force = 40;
		if (_player.GetFace() == Face.Left)
		{
			_rigid.AddForce(new Vector3(-force, 0), ForceMode2D.Impulse);
		}
		else if (_player.GetFace() == Face.Right)
		{
			_rigid.AddForce(new Vector3(force, 0), ForceMode2D.Impulse);
		}
		_g = _rigid.gravityScale;
		_rigid.gravityScale = 0;
		_seAnim.Play(Consts.PREFAB_SE_SkillRaceterShadowStrike, 0, 0);
	}

	public override void OnLeave()
	{
		_duration = _rawDura;
		_rigid.gravityScale = _g;
		_attacked.Clear();
		RefreshCoolDown();
	}
}
