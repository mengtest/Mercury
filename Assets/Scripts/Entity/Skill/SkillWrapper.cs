using System;

public class SkillWrapper
{
    private readonly FSMSystem<AbstractSkill> _skills = new FSMSystem<AbstractSkill>();
    private readonly ISkillable _skillHolder;

    public FSMSystem<AbstractSkill> FSMSystem => _skills;

    public SkillWrapper(ISkillable holder, AbstractSkill defaultSkill)
    {
        _skillHolder = holder;
        _skills.AddState(defaultSkill);
        _skills.CurrentState = defaultSkill;
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