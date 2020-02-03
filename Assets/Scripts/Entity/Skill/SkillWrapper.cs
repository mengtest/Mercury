using System;

[Obsolete("完全没啥用")]
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

    public void AddSkill(IFSMState skill) { _skills.AddState(skill ?? throw new ArgumentNullException()); }

    public bool RemoveSkill(Type skillType) { return _skills.RemoveState(skillType); }

    public void UseSkill(Type skillType) { _skills.SwitchState(skillType); }

    public void UseSkill<T>(out T skill) where T : class, IFSMState
    {
        _skills.SwitchState(typeof(T), out var state);
        skill = state as T;
    }

    public void OnUpdate() { _skills.OnUpdate(); }
}