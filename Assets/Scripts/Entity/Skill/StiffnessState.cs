﻿using UnityEngine;

/// <summary>
/// 硬直状态
/// </summary>
[EventSubscriber]
public class StiffnessState : AbstractSkill
{
    public override AssetLocation RegisterName { get; } = Consts.SkillStiffness;

    private float _expireTime;

    /// <summary>
    /// 直接set n秒后恢复默认状态
    /// </summary>
    public float ExpireTime { get => _expireTime; set => _expireTime = value + Time.time; }

    public StiffnessState(ISkillable user) : base(user) { }

    public override bool CanEnter() { return true; }

    public override void OnEnter()
    {
        var move = User.GetProperty<MoveCapability>();
        move.canMove = false;
    }

    public override void OnUpdate()
    {
        if (Time.time > _expireTime)
        {
            User.UseSkill(Consts.SkillNormal);
        }
    }

    public override void OnLeave()
    {
        var move = User.GetProperty<MoveCapability>();
        move.canMove = true;
    }

    [Subscribe]
    public static void OnRegister(object sender, RegisterEvent.AfterAuto e)
    {
        e.manager.Register(SkillEntry.Create()
            .SetRegisterName(Consts.SkillStiffness)
            .SetSkillType<StiffnessState>()
            .Build());
    }
}