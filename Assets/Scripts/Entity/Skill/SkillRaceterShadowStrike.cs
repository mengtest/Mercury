using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// 斩影
/// </summary>
public class SkillRaceterShadowStrike : AbstractSkill
{
    private float _duration;

    private readonly float _rawDura;

    private GameObject _se;
    private Animator _seAnim;
    private readonly EntityRaceter _player;
    private float _g;
    private SkillObject _seColl;
    private readonly HashSet<IAttackable> _attacked = new HashSet<IAttackable>();
    private readonly MoveCapability _playerMove;
    private float _magic;
    private float _lastMagic;

    public SkillRaceterShadowStrike(ISkillable holder) : base(holder, 0)
    {
        _player = holder as EntityRaceter;
        GetSpecialEffect();
        _rawDura = GetClipLength(_seAnim, Consts.PREFAB_SE_SkillRaceterShadowStrike);
        _duration = _rawDura;
        _playerMove = _player.GetProperty<MoveCapability>();
        GameManager.Instance.RecycleEffect(_se);
        _se = null;
        _seAnim = null;
        _seColl = null;
    }

    private void GetSpecialEffect()
    {
        _se = GameManager.Instance.GetEffect(Consts.PREFAB_SE_SkillRaceterShadowStrike);
        _seAnim = _se.GetComponent<Animator>();
        _seColl = _se.GetComponent<SkillObject>();
    }

    public override bool CanEnter() { return CurrentSkill().GetType() == typeof(NormalState) && IsCoolDown(); }

    public override void OnAct()
    {
        _duration -= Time.deltaTime;
        if (_duration <= 0)
        {
            EnterStiffness(200);
            return;
        }

        var v = new Vector2(_lastMagic - _magic, 0);
        _player.Move(v);
        _lastMagic = _magic;

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
        _player.Velocity = Vector2.zero;
        GetSpecialEffect();
        _playerMove.canMove = false;
        var transform = _player.transform;
        if (_player.GetFace() == Face.Left)
        {
            DifferentDir(_se, transform, 180, -1);
        }
        else if (_player.GetFace() == Face.Right)
        {
            DifferentDir(_se, transform, 0, 1);
        }

        _seAnim.Play(Consts.PREFAB_SE_SkillRaceterShadowStrike, 0, 0);
    }

    private void DifferentDir(GameObject se, Transform t, float y, int coe)
    {
        se.transform.eulerAngles = new Vector2(0, y);
        se.transform.position = t.position + new Vector3(3 * coe, -0.2f);
        _magic = 0f;
        _lastMagic = 0f;
        DOTween.To(() => _magic, v => _magic = v, 4f * -coe, _rawDura).SetEase(Ease.OutExpo);
    }

    public override void OnLeave()
    {
        _playerMove.canMove = true;
        _playerMove.velocity = Vector2.zero;
        _duration = _rawDura;
        _attacked.Clear();
        RefreshCoolDown();
        GameManager.Instance.RecycleEffect(_se);
        _se = null;
        _seAnim = null;
        _seColl = null;
    }
}