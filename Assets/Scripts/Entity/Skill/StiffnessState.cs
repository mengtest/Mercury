using UnityEngine;

/// <summary>
/// 硬直状态
/// </summary>
public class StiffnessState : AbstractSkill
{
    private readonly MoveCapability _move;

    public float Duration { get; set; }

    public StiffnessState(ISkillable holder) : base(holder, 0)
    {
        if (holder is Entity e)
        {
            _move = e.GetProperty<MoveCapability>();
        }
    }

    public override bool CanEnter() { return true; }

    public override void OnAct()
    {
        Duration -= Time.deltaTime * 1000;
        if (Duration <= 0)
        {
            skillHolder.UseSkill<NormalState>();
        }
    }

    public override void OnEnter() { _move.canMove = false; }

    public override void OnLeave() { _move.canMove = true; }
}