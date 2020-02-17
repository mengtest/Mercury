using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 烈
/// </summary>
[AutoRegister("raceter_flash_cut", new[] {"skill.raceter_flash_cut"})]
public class SkillRaceterFlashCut : AbstractSkill
{
    private readonly EntityRaceter _raceter;
    private readonly SwordResolve _swordResolve;
    private readonly MoveCapability _move;
    private readonly Stack<GameObject> _goPool;
    private readonly List<GameObject> _activeGo;
    private float _animLength;
    private float _animEndTime;
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
        _goPool = new Stack<GameObject>(8);
        _activeGo = new List<GameObject>(8);
    }

    public override void Init()
    {
        var tempGo = GetObjFromPool();
        var skillAnim = tempGo.GetComponent<Animator>();
        _animLength = skillAnim.AnimClipLength(Consts.GetAnimClip("raceter_flash_cut"));
        RecycleObj(tempGo);
    }

    public override bool CanEnter() { return _cdExpireTime <= Time.time && System.CurrentState.RegisterName.Equals(Consts.SkillNormal); }

    public override void OnEnter() //TODO:随机特效位置和数量
    {
        _move.canMove = false;
        _raceter.Velocity = Vector2.zero;
        foreach (var entity in _raceter.HasWindMarkBuff.ToArray())
        {
            var buf = entity as IBuffable;
            var atk = entity as IAttackable;
            var wm = buf.GetBuff(Consts.BuffWindMark);
            atk.UnderAttack(_raceter.DealDamage(_raceter.CalculateDamage(Damage * wm.intensity, DamageType.True), atk));
            var anim = GetObjFromPool();
            anim.gameObject.Show();
            anim.transform.position = entity.transform.position;
            _activeGo.Add(anim);
            buf.RemoveBuff(Consts.BuffWindMark);
        }

        _raceter.HasWindMarkBuff.Clear();
        _animEndTime = Time.time + _animLength;
    }

    public override void OnUpdate()
    {
        if (Time.time >= _animEndTime)
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
        _activeGo.ForEach(RecycleObj);
        _activeGo.Clear();
        _swordResolve.Retract();
    }

    private GameObject GetObjFromPool()
    {
        if (_goPool.Count != 0)
        {
            return _goPool.Pop();
        }

        var t = AssetManager.Instance.LoadedAssets[RegisterName.ToString()].Instantiate();
        t.transform.parent = _raceter.SkillCollection.transform;
        return t;
    }

    private void RecycleObj(GameObject go)
    {
        go.Hide();
        _goPool.Push(go);
    }
}