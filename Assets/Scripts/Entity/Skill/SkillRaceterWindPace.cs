using System;
using UnityEngine;

[EventSubscriber]
public class SkillRaceterWindPace : AbstractSkill
{
    private readonly EntityRaceter _raceter;
    private float _cdExpireTime;

    private bool IsBuffActive => _raceter.HasBuff(Consts.BuffWindPace);

    public float Cd { get; set; } = 0;

    public override AssetLocation RegisterName { get; } = Consts.SkillRaceterWindPace;

    public SkillRaceterWindPace(ISkillable user) : base(user)
    {
        if (!(user is EntityRaceter raceter))
        {
            throw new InvalidOperationException();
        }

        _raceter = raceter;
    }

    public override bool CanEnter()
    {
        if (IsBuffActive) //有Buff
        {
            return true;
        }

        return _cdExpireTime <= Time.time && System.CurrentState.RegisterName.Equals(Consts.SkillNormal);
    }

    public override void OnEnter()
    {
        //Debug.Log($"buff state:{IsBuffActive}");
        if (IsBuffActive)
        {
            _raceter.RemoveBuff(Consts.BuffWindPace);
        }
        else
        {
            _raceter.AddBuff(EntityUtility.GetBuffDot(Consts.BuffWindPace, _raceter, 0.5f, -1, 5));
        }
    }

    public override void OnUpdate() { _raceter.UseSkill(Consts.SkillNormal); }

    public override void OnLeave() { _cdExpireTime = Time.time + Cd; }

    [Subscribe]
    private static void OnRegister(object sender, RegisterEvent.AfterAuto e)
    {
        e.manager.Register(SkillEntry.Create()
            .SetRegisterName(Consts.SkillRaceterWindPace)
            .SetSkillType<SkillRaceterWindPace>()
            .Build());
    }
}