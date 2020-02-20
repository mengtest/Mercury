using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// 狂风剑刃
/// </summary>
[AutoRegister("raceter_blade_wave", "skill.raceter_blade_wave")]
public class SkillRaceterBladeWave : AbstractSkill
{
    /// <summary>
    /// 只有raceter可以持有该技能
    /// </summary>
    private readonly EntityRaceter _raceter;

    /// <summary>
    /// 剑意
    /// </summary>
    private readonly SwordResolve _swordResolve;

    /// <summary>
    /// 运动数据
    /// </summary>
    private readonly MoveCapability _move;

    /// <summary>
    /// 剑气池，是个栈
    /// </summary>
    private readonly Stack<EntityFlightProp> _pool;

    /// <summary>
    /// 剑气预制体
    /// </summary>
    [Inject] private Asset _prefab = null;

    /// <summary>
    /// 使用技能后计算出的伤害
    /// </summary>
    private Damage _activeDmg;

    /// <summary>
    /// cd到期的时间
    /// </summary>
    private float _cdExpireTime;

    /// <summary>
    /// 发射剑气数量
    /// </summary>
    private int _lunchCount;

    /// <summary>
    /// 下次发射剑气时间
    /// </summary>
    private float _nextLunchTime;

    /// <summary>
    /// 发射剑气的方向
    /// </summary>
    private int _lunchDir;

    public float Cd { get; set; } = 0;
    public float Damage { get; set; } = 150;

    /// <summary>
    /// 发射时间间隔
    /// </summary>
    public float LunchInterval { get; set; } = 0.3f;

    /// <summary>
    /// 后摇时间
    /// </summary>
    public float StiffnessTime { get; set; } = 0.2f;

    /// <summary>
    /// 剑气飞行速度
    /// </summary>
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

    public override bool CanEnter()
    {
        //当cd读条完后，且目前没有使用任何技能，才能使用该技能
        return _cdExpireTime <= Time.time && System.CurrentState.RegisterName.Equals(Consts.SkillNormal);
    }

    public override void OnEnter()
    {
        _move.canMove = false; //发射剑气时无法移动
        _raceter.Velocity = Vector2.zero; //发射剑气时速度置0
        if (!_swordResolve.swordState) //收刀时
        {
            _lunchCount = _swordResolve.resolve / 20 + 1; //每20剑意发射数量+1
        }
        else
        {
            _lunchCount = 1;
        }

        _activeDmg = _raceter.CalculateDamage(Damage, DamageType.Physics); //计算伤害
        _swordResolve.PullSword(); //拔刀
        _lunchDir = _raceter.GetFace() == Face.Left ? -1 : 1; //根据raceter面朝的方向计算剑气发射方向
        _nextLunchTime = Time.time; //下次发射时间就是现在
    }

    public override void OnUpdate()
    {
        if (Time.time >= _nextLunchTime) //到发射时间了
        {
            Lunch(); //biu biu biu~
            _nextLunchTime += LunchInterval; //计算下次发射时间
            _lunchCount -= 1; //发射数量-1
        }

        if (_lunchCount <= 0) //射 完之后进入贤者模式（后摇）
        {
            _raceter.UseSkill(Consts.SkillStiffness, out var skill);
            ((StiffnessState) skill).ExpireTime = StiffnessTime;
        }
    }

    public override void OnLeave()
    {
        _cdExpireTime = Time.time + Cd; //计算cd读条完时的时间
        _move.canMove = true; //解放移动
        _raceter.Velocity = Vector2.zero;
    }

    /// <summary>
    /// 获取一个剑气GameObject
    /// </summary>
    private EntityFlightProp GetObjFromPool()
    {
        if (_pool.Count != 0) //如果剑气池中有剑气
        {
            return _pool.Pop(); //栈中弹出一个剑气
        }

        var obj = _prefab.Instantiate(); //没有剑气的话，实例化一个
        var flight = obj.AddComponent<EntityFlightProp>(); //给剑气加上飞行道具
        obj.transform.parent = _raceter.SkillCollection.transform; //设置剑气的父物体是技能的集合
        return flight;
    }

    /// <summary>
    /// 发射剑气
    /// </summary>
    private void Lunch()
    {
        var flight = GetObjFromPool();
        flight.OnDead += OnObjDead; //添加死亡时会调用的函数
        var dir = _lunchDir; //生成发射方向的临时变量
        //飞行道具每帧调用，剑气会沿着方向和速度前进
        flight.OnUpdateAction += o => o.transform.Translate(new Vector3(dir * FlightSpeed * Time.deltaTime, 0, 0));
        flight.OnTriggerEnter += OnTriggerEvent; //当有collider2d碰到剑气时触发
        flight.OnTriggerStay += OnTriggerEvent; //当有collider2d碰到剑气时触发
        var flightTrans = flight.transform; //获取剑气的transform组件
        var scale = flightTrans.localScale; //获取剑气的变换大小
        var z = _raceter.SkillCollection.transform.position.z; //获取所有技能位置的z轴
        var pos = _raceter.transform.position; //raceter位置
        flightTrans.localScale = new Vector3(math.abs(scale.x) * _lunchDir, scale.y, scale.z); //当scale.x小于0时，剑气会沿y轴旋转180度
        flightTrans.position = new Vector3(pos.x, pos.y, z); //设置剑气的位置
        flight.gameObject.Show(); //显示剑气
    }

    /// <summary>
    /// 当有collider2d碰到剑气时触发
    /// </summary>
    /// <param name="coll">剑气的collider2d</param>
    /// <param name="o">飞行道具组件</param>
    /// <exception cref="ArgumentException"></exception>
    private void OnTriggerEvent(Collider2D coll, EntityFlightProp o)
    {
        if (!coll.CompareTag(Consts.Entity)) //如果碰到的不是实体
        {
            return;
        }

        var entity = coll.GetComponent<Entity>(); //获取实体组件
        if (entity.EntityType != EntityType.Enemy) //如果实体不是敌人
        {
            return;
        }

        if (!(entity is IAttackable attackable)) //如果实体不能被攻击
        {
            throw new ArgumentException("未实现IAttackable却是Enemy"); //炸裂
        }

        attackable.UnderAttack(_raceter.DealDamage(_activeDmg, attackable)); //攻击！
        OnObjDead(o); //剑气死亡
    }

    /// <summary>
    /// 飞行道具，也就是剑气死亡时调用该函数
    /// </summary>
    /// <param name="o">飞行道具组件</param>
    private void OnObjDead(EntityFlightProp o)
    {
        _pool.Push(o); //入栈
        o.Clear(); //清理飞行道具组件的数据
        o.gameObject.Hide(); //隐藏剑气
    }
}