/// <summary>
/// 普通状态
/// </summary>
[AutoRegister("normal")]
public class NormalState : AbstractSkill
{
    public override AssetLocation RegisterName { get; } = Consts.SkillNormal;

    public NormalState(ISkillable user) : base(user) { }

    public override void Init() { }

    public override bool CanEnter() { return true; }

    public override void OnEnter() { }

    public override void OnUpdate() { }

    public override void OnLeave() { }
}