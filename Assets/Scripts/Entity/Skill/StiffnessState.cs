using UnityEngine;

/// <summary>
/// 硬直状态
/// </summary>
public class StiffnessState : AbstractSkill
{
    public override AssetLocation RegisterName { get; } = Consts.SkillStiffness;
    public override void Init(SkillStack stack) { }

    public override bool CanEnter(SkillStack stack) { return true; }

    public override void OnEnter(SkillStack stack)
    {
        var move = stack.user.GetProperty<MoveCapability>();
        move.canMove = false;
    }

    public override void OnUpdate(SkillStack stack)
    {
        var property = stack.property as float[];
        var expireTime = property[0];
        if (Time.time > expireTime)
        {
            stack.user.UseSkill(new AssetLocation(Consts.Mercury, Consts.Skill, "Normal").ToString());
        }
    }

    public override void OnLeave(SkillStack stack)
    {
        var move = stack.user.GetProperty<MoveCapability>();
        move.canMove = true;
    }
}