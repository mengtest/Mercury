using System.Collections.Generic;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// 斩影
/// </summary>
public class SkillRaceterShadowStrike : SkillObject
{
    public Animator skillAnimator;
    public EntityRaceter raceter;
    private MoveCapability _playerMove;
    private SwordResolve _swordResolve;
    private Damage _damage;
    private float _animLength;
    private float _duration;
    private float _move;
    private float _lastMove;
    private readonly HashSet<IAttackable> _attacked = new HashSet<IAttackable>();

    public override FSMSystem System => raceter.SkillFsmSystem;

    private void Awake() { gameObject.Hide(); }

    public override void Init()
    {
        raceter = transform.parent.GetComponent<EntityRaceter>();
        _playerMove = raceter.GetProperty<MoveCapability>();
        _swordResolve = raceter.GetProperty<SwordResolve>();
        _animLength = SkillUtility.GetClipLength(skillAnimator, Consts.PREFAB_SE_SkillRaceterShadowStrike);
        _duration = _animLength;
        transform.parent = raceter.SkillObjCollection.transform;
    }

    public override bool CanEnter() { return System.CurrentState.GetType() == typeof(NormalState) && IsCoolDown(); }

    public override void OnAct()
    {
        _duration -= Time.deltaTime;
        if (_duration <= 0)
        {
            EnterStiffness(200);
            return;
        }

        var v = new Vector2(_lastMove - _move, 0);
        raceter.Move(v);
        _lastMove = _move;
    }

    public override void OnEnter()
    {
        _damage = raceter.CalculateDamage(95, DamageType.Physics);
        _swordResolve.PullSword();
        gameObject.Show();
        raceter.Velocity = Vector2.zero;
        _playerMove.canMove = false;
        DifferentDir((int) raceter.GetFace());
        skillAnimator.Play(Consts.PREFAB_SE_SkillRaceterShadowStrike, 0, 0);
    }

    private void DifferentDir(int coe)
    {
        var t = transform;
        var scale = t.localScale;
        t.localScale = new Vector3(math.abs(scale.x) * coe, scale.y, scale.z);
        t.position = raceter.transform.position + new Vector3(3 * coe, -0.2f);
        _move = 0f;
        _lastMove = 0f;
        DOTween.To(() => _move, v => _move = v, 4f * -coe, _animLength).SetEase(Ease.OutExpo);
    }

    public override void OnLeave()
    {
        _playerMove.canMove = true;
        _playerMove.velocity = Vector2.zero;
        _duration = _animLength;
        _attacked.Clear();
        RefreshCoolDown();
        gameObject.Hide();
    }

    protected override void OnTriggerEnterEvent(Collider2D coll)
    {
        if (!coll.CompareTag(Consts.TAG_Entity))
        {
            return;
        }

        var e = coll.GetComponent<Entity>();
        if (e.EntityType != EntityType.Enemy)
        {
            return;
        }

        if (!(e is IAttackable attackable))
        {
            return;
        }

        if (_attacked.Contains(attackable))
        {
            return;
        }

        attackable.UnderAttack(raceter.DealDamage(_damage, attackable));
        _attacked.Add(attackable);
    }
}