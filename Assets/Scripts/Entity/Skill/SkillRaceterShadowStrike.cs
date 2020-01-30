using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 斩影
/// </summary>
public class SkillRaceterShadowStrike : AbstractSkill
{
    private float _duration;
    private readonly float _rawDura;
    private readonly Rigidbody2D _rigid;
    private GameObject _se;
    private Animator _seAnim;
    private readonly EntityPlayer _player;
    private float _g;
    private SkillObject _seColl;
    private readonly HashSet<IAttackable> _attacked = new HashSet<IAttackable>();
    private readonly MoveCapability _playerMove;

    public SkillRaceterShadowStrike(ISkillable holder) : base(holder, 12)
    {
        _player = holder as EntityPlayer;
        _rigid = _player.GetComponent<Rigidbody2D>();
        SetSpecialEffect();
        _rawDura = GetClipLength(_seAnim, Consts.PREFAB_SE_SkillRaceterShadowStrike) * 1000;
        _duration = _rawDura;
        _playerMove = _player.GetProperty<MoveCapability>();
        GameManager.Instance.RecycleEffect(_se);
        _se = null;
        _seAnim = null;
        _seColl = null;
    }

    private void SetSpecialEffect()
    {
        _se = GameManager.Instance.GetEffect(Consts.PREFAB_SE_SkillRaceterShadowStrike);
        _seAnim = _se.GetComponent<Animator>();
        _seColl = _se.GetComponent<SkillObject>();
    }

    public override bool CanEnter() { return CurrentSkill().GetType() == typeof(NormalState) && IsCoolDown(); }

    public override void OnAct()
    {
        _duration -= Time.deltaTime * 1000;
        if (_duration <= 0)
        {
            EnterStiffness(200);
            return;
        }

        var playerVelocity = _rigid.velocity;
        _se.transform.position = _player.transform.position;
        _rigid.AddForce(new Vector2(-playerVelocity.x * (_rawDura - _duration / _rawDura) * 0.01f, 0));
        if (_seColl.Contact)
        {
            var e = _seColl.Contact.GetComponent<Entity>();
            if (e.EntityType != EntityType.Enemy)
            {
                return;
            }

            if (e is IAttackable attackable)
            {
                if (!_attacked.Contains(attackable))
                {
                    attackable.UnderAttack(_player.DealDamage(_player.CalculateDamage(95, DamageType.Physics),
                        attackable));
                    _attacked.Add(attackable);
                }
            }
        }
    }

    public override void OnEnter()
    {
        SetSpecialEffect();
        _playerMove.canMove = false;
        var seR = _se.transform.eulerAngles;
        var transform = _player.transform;
        var pR = transform.eulerAngles;
        _se.transform.eulerAngles = new Vector3(seR.x, pR.y - 180, seR.z);
        _se.transform.position = transform.position;
        _rigid.velocity = Vector2.zero;
        const int force = 40;
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
        _playerMove.canMove = true;
        _duration = _rawDura;
        _rigid.gravityScale = _g;
        _attacked.Clear();
        RefreshCoolDown();
        GameManager.Instance.RecycleEffect(_se);
        _se = null;
        _seAnim = null;
        _seColl = null;
    }
}