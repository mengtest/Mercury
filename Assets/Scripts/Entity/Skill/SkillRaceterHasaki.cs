using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillRaceterHasaki : AbstractSkill
{


	public SkillRaceterHasaki(ISkillable holder) : base(holder)
	{
	}

	public override bool CanEnter(IFSMState current)
	{
		return current.GetType() == typeof(NormalState);
	}

	public override void OnAct()
	{
		
	}

	public override void OnEnter()
	{
		
	}

	public override void OnLeave()
	{
		
	}
}
