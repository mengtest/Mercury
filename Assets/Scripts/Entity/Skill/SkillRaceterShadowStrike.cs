using System;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// 斩影
/// </summary>
public class SkillRaceterShadowStrike : AbstractSkill
{
    private readonly EntityRaceter _raceter;
    private readonly SwordResolve _swordResolve;
    private readonly MoveCapability _move;
    private readonly HashSet<Collider2D> _attacked = new HashSet<Collider2D>();
    private float _cdExpireTime;
    private GameObject _skillObj;
    private Animator _skillAnim;
    private float _animLength;
    private Damage _activeDmg;
    private float _raceterMove;
    private float _raceterLastMove;
    private float _animEndTime;

    public override AssetLocation RegisterName { get; } = Consts.SkillRaceterShadowStrike;
    public float Cd { get; set; } = 0;
    public float Damage { get; set; } = 95;
    public float AnimSpeed { get; set; } = 1;
    public float StiffnessTime { get; set; } = 0.2f;

    public SkillRaceterShadowStrike(ISkillable user) : base(user)
    {
        if (!(user is EntityRaceter raceter))
        {
            throw new InvalidOperationException();
        }

        _raceter = raceter;
        _swordResolve = raceter.GetProperty<SwordResolve>();
        _move = raceter.GetProperty<MoveCapability>();
    }

    public override void Init()
    {
        _skillObj = AssetManager.Instance.LoadedAssets[RegisterName.ToString()].Instantiate();
        _skillAnim = _skillObj.GetComponent<Animator>();
        _animLength = _skillAnim.AnimClipLength(Consts.GetAnimClip("raceter_shadow_strike"));
        _skillObj.transform.parent = _raceter.SkillCollection.transform;
        var callback = _skillObj.AddComponent<TriggerEventCallback>();
        callback.OnTriggerEnterEvent += OnTriggerEvent;
        callback.OnTriggerStayEvent += OnTriggerEvent;
        callback.gameObject.Hide();
    }

    public override bool CanEnter()
    {
        return _cdExpireTime <= Time.time && System.CurrentState.RegisterName.Equals(Consts.SkillNormal);
    }

    public override void OnEnter()
    {
        _move.canMove = false;
        _swordResolve.PullSword();
        _activeDmg = _raceter.CalculateDamage(Damage, DamageType.Physics);
        _raceter.Velocity = Vector2.zero;
        _skillObj.Show();
        var eTrans = _raceter.transform;
        var pos = eTrans.position;
        var scale = _skillObj.transform.localScale;
        var dir = _raceter.GetFace() == Face.Left ? -1 : 1;
        var z = _raceter.SkillCollection.transform.position.z;
        _skillObj.transform.position = new Vector3(pos.x + 3 * dir, pos.y, z);
        _skillObj.transform.localScale = new Vector3(math.abs(scale.x) * dir, scale.y, scale.z);
        _raceterMove = 0;
        _raceterLastMove = 0;
        DOTween.To(() => _raceterMove, v => _raceterMove = v, 4f * -dir, _animLength / AnimSpeed).SetEase(Ease.OutExpo);
        _skillAnim.speed = AnimSpeed;
        _animEndTime = _animLength / AnimSpeed + Time.time;
    }

    public override void OnUpdate()
    {
        if (Time.time >= _animEndTime)
        {
            _raceter.UseSkill(Consts.SkillStiffness.ToString(), out var skill);
            ((StiffnessState) skill).ExpireTime = StiffnessTime;
            return;
        }

        var v = new Vector2(_raceterLastMove - _raceterMove, 0);
        _raceter.Move(v);
        _raceterLastMove = _raceterMove;
    }

    public override void OnLeave()
    {
        _skillObj.Hide();
        _cdExpireTime = Time.time + Cd;
        _move.canMove = true;
        _raceter.Velocity = Vector2.zero;
        _attacked.Clear();
    }

    private void OnTriggerEvent(Collider2D coll)
    {
        if (!coll.CompareTag(Consts.Entity))
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

        if (_attacked.Contains(coll))
        {
            return;
        }

        attackable.UnderAttack(_raceter.DealDamage(_activeDmg, attackable));
        _attacked.Add(coll);
    }
}