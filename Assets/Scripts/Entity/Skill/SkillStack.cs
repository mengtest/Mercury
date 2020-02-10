public class SkillStack : IFSMState
{
    public readonly ISkillable user;
    public readonly AbstractSkill skill;
    public object property;

    public AssetLocation RegisterName => skill.RegisterName;
    public FSMSystem System => user.SkillFsmSystem;

    public SkillStack(ISkillable user, AbstractSkill skill, object property = null)
    {
        this.user = user;
        this.skill = skill;
        this.property = property;
    }

    public void Init() { skill.Init(this); }

    public bool CanEnter() { return skill.CanEnter(this); }

    public void OnEnter() { skill.OnEnter(this); }

    public void OnUpdate() { skill.OnUpdate(this); }

    public void OnLeave() { skill.OnLeave(this); }
}