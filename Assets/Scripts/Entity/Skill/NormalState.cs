using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalState : AbstractSkill
{
	public NormalState(ISkillable holder) : base(holder, 0)
	{
	}

	public override bool CanEnter(IFSMState current)
	{
		return true;
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
