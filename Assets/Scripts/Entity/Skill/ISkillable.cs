using System;

public interface ISkillable
{
	FSMSystem<AbstractSkill> FSMSystem { get; }

	void AddSkill(AbstractSkill skill);

	bool RemoveSkill(Type skillType);

	void UseSkill(Type skillType);

	void UseSkill<T>(out T skill) where T : AbstractSkill;

	void OnUpdate();
}
