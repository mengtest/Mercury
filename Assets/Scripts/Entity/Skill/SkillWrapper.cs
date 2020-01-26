using System;

public class SkillWrapper
{
    private readonly FSMSystem _skills;
    private readonly ISkillable _skillHolder;

    public FSMSystem FSMSystem => _skills;

    public SkillWrapper(ISkillable holder, AbstractSkill defaultSkill)
    {
        _skills = new FSMSystem(defaultSkill);
        _skillHolder = holder;
    }

    public void AddSkill(AbstractSkill skill) { _skills.AddState(skill); }

    public bool RemoveSkill(Type skillType) { return _skills.RemoveState(skillType); }

    public void UseSkill(Type skillType) { _skills.SwitchState(skillType); }

    public void UseSkill<T>(out T skill) where T : AbstractSkill
    {
        _skills.SwitchState(typeof(T), out var state);
        skill = state as T;
    }

    public void OnUpdate() { _skills.OnUpdate(); }
}