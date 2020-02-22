/// <summary>
/// 普通状态
/// </summary>
[EventSubscriber]
public class NormalState : AbstractSkill
{
    public override AssetLocation RegisterName { get; } = Consts.SkillNormal;

    public NormalState(ISkillable user) : base(user) { }

    public override bool CanEnter() { return true; }

    public override void OnEnter() { }

    public override void OnUpdate() { }

    public override void OnLeave() { }

    [Subscribe]
    private static void OnRegister(object sender, RegisterEvent.AfterAuto e)
    {
        e.manager.Register(SkillEntry.Create()
            .SetRegisterName(Consts.SkillNormal)
            .SetSkillType<NormalState>()
            .Build());
    }
}