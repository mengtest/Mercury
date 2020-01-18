using System.Linq;
using UnityEngine;

public abstract class AbstractSkill : IFSMState
{
	protected readonly ISkillable _skillHolder;
	protected readonly float _cd;
	protected float _lastUse = float.MinValue;

	protected AbstractSkill(ISkillable holder, float cd)
	{
		_skillHolder = holder;
		_cd = cd;
	}

	public abstract bool CanEnter(IFSMState current);
	public abstract void OnAct();
	public abstract void OnEnter();
	public abstract void OnLeave();

	protected bool IsCoolDown()
	{
		return Time.time - _lastUse >= _cd;
	}

	protected void RefreshCoolDown()
	{
		_lastUse = Time.time;
	}

	public static float GetClipLength(Animator animator, string clipName)
	{
		return animator.runtimeAnimatorController.animationClips.First((clip) => clip.name == clipName).length;
	}
}
