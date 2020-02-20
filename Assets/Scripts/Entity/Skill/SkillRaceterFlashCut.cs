using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 烈
/// </summary>
[AutoRegister("raceter_flash_cut", "skill.raceter_flash_cut")]
public class SkillRaceterFlashCut : AbstractSkill
{
    private readonly EntityRaceter _raceter;
    private readonly SwordResolve _swordResolve;
    private readonly MoveCapability _move;

    /// <summary>
    /// 特效池
    /// </summary>
    private readonly Stack<GameObject> _goPool;

    /// <summary>
    /// 活动中特效
    /// </summary>
    private readonly List<GameObject> _activeGo;

    /// <summary>
    /// 特效预制体
    /// </summary>
    [Inject] private Asset _prefab = null;

    /// <summary>
    /// 特效动画时间长度
    /// </summary>
    private float _animLength;

    /// <summary>
    /// 动画结束时间
    /// </summary>
    private float _animEndTime;

    /// <summary>
    /// cd结束时间
    /// </summary>
    private float _cdExpireTime;

    public float Cd { get; set; } = 0;
    public float Damage { get; set; } = 100;
    public float StiffnessTime { get; set; } = 0.2f;

    public override AssetLocation RegisterName { get; } = Consts.SkillRaceterFlashCut;

    public SkillRaceterFlashCut(ISkillable user) : base(user)
    {
        if (!(user is EntityRaceter raceter))
        {
            throw new InvalidOperationException();
        }

        _raceter = raceter;
        _swordResolve = raceter.GetProperty<SwordResolve>();
        _move = raceter.GetProperty<MoveCapability>();
        _goPool = new Stack<GameObject>(8); //初始化特效池
        _activeGo = new List<GameObject>(8); //初始化
    }

    public override void Init()
    {
        var tempGo = GetObjFromPool(); //先获取一个特效
        var skillAnim = tempGo.GetComponent<Animator>();
        _animLength = skillAnim.AnimClipLength(Consts.GetAnimClip("raceter_flash_cut")); //获取动画长度
        RecycleObj(tempGo); //回收掉
    }

    public override bool CanEnter() { return _cdExpireTime <= Time.time && System.CurrentState.RegisterName.Equals(Consts.SkillNormal); }

    public override void OnEnter() //TODO:随机特效位置和数量
    {
        _move.canMove = false; //冻住，不许走
        _raceter.Velocity = Vector2.zero;
        foreach (var entity in _raceter.HasWindMarkBuff.ToArray()) //获取持有WindMark的实体的容器的复制
        {
            var buf = entity as IBuffable;
            var atk = entity as IAttackable;
            var wm = buf.GetBuff(Consts.BuffWindMark); //获取实体持有的那个buff
            atk.UnderAttack(_raceter.DealDamage(_raceter.CalculateDamage(Damage * wm.intensity, DamageType.True), atk)); //造成伤害
            var anim = GetObjFromPool(); //获取动画
            anim.gameObject.Show(); //显示动画
            anim.transform.position = entity.transform.position; //设置动画位置
            _activeGo.Add(anim); //加入活动中动画的容器
            buf.RemoveBuff(Consts.BuffWindMark); //在实体身上移除buff
        }

        _raceter.HasWindMarkBuff.Clear(); //清空持有WindMark的实体的容器，因为buff都被移除了
        _animEndTime = Time.time + _animLength; //计算动画结束时间
    }

    public override void OnUpdate()
    {
        if (Time.time >= _animEndTime) //是否进入贤者时间
        {
            _raceter.UseSkill(Consts.SkillStiffness, out var skill);
            ((StiffnessState) skill).ExpireTime = StiffnessTime;
        }
    }

    public override void OnLeave()
    {
        _cdExpireTime = Time.time + Cd; //计算cd读条完的时间
        _move.canMove = true;
        _raceter.Velocity = Vector2.zero;
        _activeGo.ForEach(RecycleObj); //回收活动中的特效
        _activeGo.Clear(); //清空特效
        _swordResolve.Retract(); //收刀
    }

    private GameObject GetObjFromPool()
    {
        if (_goPool.Count != 0)
        {
            return _goPool.Pop();
        }

        var t = _prefab.Instantiate();
        t.transform.parent = _raceter.SkillCollection.transform;
        return t;
    }

    private void RecycleObj(GameObject go)
    {
        go.Hide();
        _goPool.Push(go);
    }
}