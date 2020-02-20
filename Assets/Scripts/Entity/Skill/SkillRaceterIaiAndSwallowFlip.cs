using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// 居合·燕返
/// </summary>
[AutoRegister("raceter_iai_swallow_flip", "skill.raceter_swallow_flip")]
public class SkillRaceterIaiAndSwallowFlip : AbstractSkill //TODO:居合等特效好了再做
{
    private readonly EntityRaceter _raceter;
    private readonly SwordResolve _swordResolve;
    private readonly MoveCapability _move;

    /// <summary>
    /// 燕返攻击的敌人
    /// </summary>
    private readonly Dictionary<Collider2D, (int, float)> _swallowAtk = new Dictionary<Collider2D, (int, float)>();

    /// <summary>
    /// 燕返特效
    /// </summary>
    [Inject] private GameObject _swallowGo = null;

    /// <summary>
    /// 燕返动画
    /// </summary>
    private Animator _swallowAnim;

    /// <summary>
    /// 燕返动画时间长度
    /// </summary>
    private float _swallowAnimLength;

    /// <summary>
    /// cd读条完毕时的时间
    /// </summary>
    private float _cdExpireTime;

    /// <summary>
    /// 伤害量
    /// </summary>
    private Damage _activeDmg;

    /// <summary>
    /// 燕返攻击间隔
    /// </summary>
    private float _swallowAtkInterval;

    /// <summary>
    /// 动画结束时间
    /// </summary>
    private float _animEndTime;

    public override AssetLocation RegisterName { get; } = Consts.SkillRaceterIaiAndSwallowFlip;
    public float Cd { get; set; } = 0;
    public float Damage { get; set; } = 60;

    /// <summary>
    /// 动画速度
    /// </summary>
    public float AnimSpeed { get; set; } = 1;

    public float StiffnessTime { get; set; } = 0.2f;

    /// <summary>
    /// 燕返攻击次数
    /// </summary>
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
        if (_swordResolve.swordState) //拔刀状态
        {
            _activeDmg = _raceter.CalculateDamage(Damage, DamageType.Physics); //计算伤害
            _swordResolve.PullSword(); //收刀
            _swallowGo.Show(); //显示燕返特效
            var swallowTrans = _swallowGo.transform; //燕返的transform
            var raceterTrans = _raceter.transform; //raceter的transform
            var dir = _raceter.GetFace() == Face.Left ? -1 : 1; //raceter面朝的方向
            var pos = raceterTrans.position; //raceter的位置
            var swallowScale = swallowTrans.localScale; //燕返的scale
            var z = _raceter.SkillCollection.transform.position.z; //技能z轴
            swallowTrans.position = new Vector3(pos.x, pos.y, z); //设置燕返的位置
            swallowTrans.localScale = new Vector3(math.abs(swallowScale.x) * dir, swallowScale.y, swallowScale.z); //设置燕返朝向
            var animTime = _swallowAnimLength / AnimSpeed; //计算动画长度
            _animEndTime = animTime + Time.time; //动画结束时间
            _swallowAtkInterval = animTime / 2.5f / SwallowAtkCount; //计算攻击间隔
        }
    }

    public override void OnUpdate()
    {
        if (!(Time.time >= _animEndTime)) //贤者时间
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
        if (!coll.CompareTag(Consts.Entity)) //是不是实体
        {
            return;
        }

        var e = coll.GetComponent<Entity>();
        if (e.EntityType != EntityType.Enemy) //是不是敌人
        {
            return;
        }

        if (!(e is IAttackable attackable)) //能不能锤
        {
            return;
        }

        if (_swallowAtk.TryGetValue(coll, out var pair)) //是否锤过的敌人
        {
            var count = pair.Item1; //已锤过的次数
            var time = pair.Item2; //上次锤的时间
            if (count >= SwallowAtkCount) //是否锤够了
            {
                return;
            }

            if (time >= _swallowAtkInterval) //是否到下次锤的时间
            {
                //是的话，再锤一次
                attackable.UnderAttack(_raceter.DealDamage(_activeDmg, attackable));
                _swallowAtk[coll] = (count + 1, 0);
            }
            else
            {
                //不是的话放过
                _swallowAtk[coll] = (count, time + Time.deltaTime);
            }
        }
        else
        {
            //锤一次
            attackable.UnderAttack(_raceter.DealDamage(_activeDmg, attackable));
            _swallowAtk.Add(coll, (1, 0));
        }
    }
}