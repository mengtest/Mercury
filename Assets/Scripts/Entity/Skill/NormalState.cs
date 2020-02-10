/// <summary>
/// 普通状态
/// </summary>
public class NormalState : AbstractSkill
{
    public override AssetLocation RegisterName { get; } = Consts.SkillNormal;
    public override void Init(SkillStack stack) { }

    public override bool CanEnter(SkillStack stack) { return true; }

    public override void OnEnter(SkillStack stack) { }

    public override void OnUpdate(SkillStack stack) { }

    public override void OnLeave(SkillStack stack) { }
}