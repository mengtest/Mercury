using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// 居合·燕返
/// </summary>
[AutoRegister("raceter_iai_swallow_flip", new[] {"skill.raceter_swallow_flip"})]
public class SkillRaceterIaiAndSwallowFlip : AbstractSkill //TODO:居合等特效好了再做
{
    private readonly EntityRaceter _raceter;
    private readonly SwordResolve _swordResolve;
    private readonly MoveCapability _move;
    private readonly Dictionary<Collider2D, (int, float)> _swallowAtk = new Dictionary<Collider2D, (int, float)>();
    [Inject] private GameObject _swallowGo = null;
    private Animator _swallowAnim;
    private float _swallowAnimLength;
    private float _cdExpireTime;
    private Damage _activeDmg;
    private float _swallowAtkInterval;
    private float _animEndTime;

    public override AssetLocation RegisterName { get; } = Consts.SkillRaceterIaiAndSwallowFlip;
    public float Cd { get; set; } = 0;
    public float Damage { get; set; } = 60;
    public float AnimSpeed { get; set; } = 1;
    public float StiffnessTime { get; set; } = 0.2f;
    public int SwallowAtkCount { get; set; } = 6;

    public SkillRaceterIaiAndSwallowFlip(ISkillable user) : base(user)
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
        _swallowAnim = _swallowGo.GetComponent<Animator>();
        _swallowAnimLength = _swallowAnim.AnimClipLength(Consts.GetAnimClip("raceter_swallow_flip"));
        _swallowGo.transform.parent = _raceter.SkillCollection.transform;
        var callback = _swallowGo.AddComponent<TriggerEventCallback>();
        callback.OnTriggerEnterEvent += OnSwallowTriggerEvent;
        callback.OnTriggerStayEvent += OnSwallowTriggerEvent;
        _swallowGo.Hide();
    }

    public override bool CanEnter() { return _cdExpireTime <= Time.time && System.CurrentState.RegisterName.Equals(Consts.SkillNormal); }

    public override void OnEnter()
    {
        _move.canMove = false;
        _raceter.Velocity = Vector2.zero;
        if (_swordResolve.swordState)
        {
            _activeDmg = _raceter.CalculateDamage(Damage, DamageType.Physics);
            _swordResolve.PullSword();
            _swallowGo.Show();
            var swallowTrans = _swallowGo.transform;
            var raceterTrans = _raceter.transform;
            var dir = _raceter.GetFace() == Face.Left ? -1 : 1;
            var pos = raceterTrans.position;
            var swallowScale = swallowTrans.localScale;
            var z = _raceter.SkillCollection.transform.position.z;
            swallowTrans.position = new Vector3(pos.x, pos.y, z);
            swallowTrans.localScale = new Vector3(math.abs(swallowScale.x) * dir, swallowScale.y, swallowScale.z);
            var animTime = _swallowAnimLength / AnimSpeed;
            _animEndTime = animTime + Time.time;
            _swallowAtkInterval = animTime / 2.5f / SwallowAtkCount;
        }
    }

    public override void OnUpdate()
    {
        if (!(Time.time >= _animEndTime))
        {
            return;
        }

        _raceter.UseSkill(Consts.SkillStiffness, out var skill);
        ((StiffnessState) skill).ExpireTime = StiffnessTime;
    }

    public override void OnLeave()
    {
        _cdExpireTime = Time.time + Cd;
        _swallowAtk.Clear();
        _swallowGo.Hide();
        _move.canMove = true;
        _raceter.Velocity = Vector2.zero;
    }

    private void OnSwallowTriggerEvent(Collider2D coll)
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

        if (_swallowAtk.TryGetValue(coll, out var pair))
        {
            var count = pair.Item1;
            var time = pair.Item2;
            if (count >= SwallowAtkCount)
            {
                return;
            }

            if (time >= _swallowAtkInterval)
            {
                attackable.UnderAttack(_raceter.DealDamage(_activeDmg, attackable));
                _swallowAtk[coll] = (count + 1, 0);
            }
            else
            {
                _swallowAtk[coll] = (count, time + Time.deltaTime);
            }
        }
        else
        {
            attackable.UnderAttack(_raceter.DealDamage(_activeDmg, attackable));
            _swallowAtk.Add(coll, (1, 0));
        }
    }
}