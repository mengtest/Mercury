using UnityEngine;

/// <summary>
/// 硬直状态
/// </summary>
public class StiffnessState : AbstractSkill
{
	public float Duration { get; set; }

	public StiffnessState(ISkillable holder) : base(holder, 0)
	{
	}

	public override bool CanEnter(IFSMState current)
	{
		return true;
	}

	public override void OnAct()
	{
		Duration -= Time.deltaTime * 1000;
		if (Duration <= 0)
		{
			_skillHolder.UseSkill(typeof(NormalState));
		}
	}

	public override void OnEnter()
	{
	}

	public override void OnLeave()
	{
	}
}
