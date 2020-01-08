public abstract class AbstractSkill : IFSMState
{
	protected readonly ISkillable _skillHolder;

	protected AbstractSkill(ISkillable holder)
	{
		_skillHolder = holder;
	}

	public abstract bool CanEnter(IFSMState current);
	public abstract void OnAct();
	public abstract void OnEnter();
	public abstract void OnLeave();
}
