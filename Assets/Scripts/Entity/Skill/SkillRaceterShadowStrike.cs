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

	public SkillRaceterShadowStrike(ISkillable holder, float animTime) : base(holder)
	{
		_rawDura = animTime;
		_duration = animTime;
		_player = holder as EntityPlayer;
		_rigid = _player.GetComponent<Rigidbody2D>();
		_se = GameManager.Instance.GetEffect(Consts.PREFAB_SE_2_1_dodge);
		_seAnim = _se.GetComponent<Animator>();
		_se.Hide();
	}

	~SkillRaceterShadowStrike()
	{
		GameManager.Instance.RecycleEffect(_se);
	}

	public override bool CanEnter(IFSMState current)
	{
		return current.GetType() == typeof(NormalState);
	}

	public override void OnAct()
	{
		_duration -= Time.deltaTime * 1000;
		_se.transform.position = _player.transform.position;
		_rigid.AddForce(new Vector2(-_rigid.velocity.x * (_rawDura - _duration / _rawDura) * 0.01f, 0));
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
		//var v = _rigid.velocity;
		//var lv = v.magnitude;
		//var force = 20 - lv;
		_rigid.velocity = Vector2.zero;
		var force = 40;
		//_rigid.velocity = new Vector2(lv, 0);
		if (pR.y == 0f)
		{
			_rigid.AddForce(new Vector3(-force, 0), ForceMode2D.Impulse);
			//_rigid.velocity += new Vector2(-force, 0);
		}
		else if (pR.y == 180f)
		{
			_rigid.AddForce(new Vector3(force, 0), ForceMode2D.Impulse);
			//_rigid.velocity += new Vector2(force, 0);
		}
		_g = _rigid.gravityScale;
		_rigid.gravityScale = 0;
		_seAnim.Play("Thunder", 0, 0);
	}

	public override void OnLeave()
	{
		_duration = _rawDura;
		_rigid.gravityScale = _g;
	}
}
