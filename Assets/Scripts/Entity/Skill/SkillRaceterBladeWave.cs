using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// 狂风剑刃
/// </summary>
[AutoRegister("raceter_blade_wave", new[] {"skill.raceter_blade_wave"})]
public class SkillRaceterBladeWave : AbstractSkill
{
    private readonly EntityRaceter _raceter;
    private readonly SwordResolve _swordResolve;
    private readonly MoveCapability _move;
    private readonly Stack<EntityFlightProp> _pool;
    [Inject] private Asset _prefab = null;
    private Damage _activeDmg;
    private float _cdExpireTime;
    private int _lunchCount;
    private float _nextLunchTime;
    private int _lunchDir;

    public float Cd { get; set; } = 0;
    public float Damage { get; set; } = 150;
    public float LunchInterval { get; set; } = 0.3f;
    public float StiffnessTime { get; set; } = 0.2f;
    public float FlightSpeed { get; set; } = 3;

    public override AssetLocation RegisterName { get; } = Consts.SkillRaceterBladeWave;

    public SkillRaceterBladeWave(ISkillable user) : base(user)
    {
        if (!(user is EntityRaceter raceter))
        {
            throw new InvalidOperationException();
        }

        _raceter = raceter;
        _swordResolve = raceter.GetProperty<SwordResolve>();
        _move = raceter.GetProperty<MoveCapability>();
        _pool = new Stack<EntityFlightProp>(5);
    }

    public override void Init() { }

    public override bool CanEnter() { return _cdExpireTime <= Time.time && System.CurrentState.RegisterName.Equals(Consts.SkillNormal); }

    public override void OnEnter()
    {
        _move.canMove = false;
        _raceter.Velocity = Vector2.zero;
        if (!_swordResolve.swordState)
        {
            _lunchCount = _swordResolve.resolve / 20 + 1;
        }
        else
        {
            _lunchCount = 1;
        }

        _activeDmg = _raceter.CalculateDamage(Damage, DamageType.Physics);
        _swordResolve.PullSword();
        _lunchDir = _raceter.GetFace() == Face.Left ? -1 : 1;
        _nextLunchTime = Time.time;
    }

    public override void OnUpdate()
    {
        if (Time.time >= _nextLunchTime)
        {
            Lunch();
            _nextLunchTime += LunchInterval;
            _lunchCount -= 1;
        }

        if (_lunchCount <= 0)
        {
            _raceter.UseSkill(Consts.SkillStiffness, out var skill);
            ((StiffnessState) skill).ExpireTime = StiffnessTime;
        }
    }

    public override void OnLeave()
    {
        _cdExpireTime = Time.time + Cd;
        _move.canMove = true;
        _raceter.Velocity = Vector2.zero;
    }

    public EntityFlightProp GetObjFromPool()
    {
        if (_pool.Count != 0)
        {
            return _pool.Pop();
        }

        var obj = _prefab.Instantiate();
        var flight = obj.AddComponent<EntityFlightProp>();
        obj.transform.parent = _raceter.SkillCollection.transform;
        return flight;
    }

    private void Lunch()
    {
        var flight = GetObjFromPool();
        flight.OnDead += OnObjDead;
        var dir = _lunchDir;
        flight.OnUpdateAction += o => o.transform.Translate(new Vector3(dir * FlightSpeed * Time.deltaTime, 0, 0));
        flight.OnTriggerEnter += OnTriggerEvent;
        flight.OnTriggerStay += OnTriggerEvent;
        var flightTrans = flight.transform;
        var scale = flightTrans.localScale;
        var z = _raceter.SkillCollection.transform.position.z;
        var pos = _raceter.transform.position;
        flightTrans.localScale = new Vector3(math.abs(scale.x) * _lunchDir, scale.y, scale.z);
        flightTrans.position = new Vector3(pos.x, pos.y, z);
        flight.gameObject.Show();
    }

    private void OnTriggerEvent(Collider2D coll, EntityFlightProp o)
    {
        if (!coll.CompareTag(Consts.Entity))
        {
            return;
        }

        var entity = coll.GetComponent<Entity>();
        if (entity.EntityType != EntityType.Enemy)
        {
            return;
        }

        if (!(entity is IAttackable attackable))
        {
            throw new ArgumentException("未实现IAttackable却是Enemy");
        }

        attackable.UnderAttack(_raceter.DealDamage(_activeDmg, attackable));
        OnObjDead(o);
    }

    private void OnObjDead(EntityFlightProp o)
    {
        _pool.Push(o);
        o.Clear();
        o.gameObject.Hide();
    }
}