using System;

public class SkillWrapper
{
	private readonly FSMSystem<AbstractSkill> _skills = new FSMSystem<AbstractSkill>();
	private readonly ISkillable _skillHolder;

	public FSMSystem<AbstractSkill> FSMSystem => _skills;

	public SkillWrapper(ISkillable holder)
	{
		_skillHolder = holder;
	}

	public void AddSkill(AbstractSkill skill)
	{
		_skills.AddState(skill);
	}

	public bool RemoveSkill(Type skillType)
	{
		return _skills.RemoveState(skillType);
	}

	public void UseSkill(Type skillType)
	{
		_skills.SwitchState(skillType);
	}

	public void OnUpdate()
	{
		_skills.OnUpdate();
	}
}
