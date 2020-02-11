public abstract class AbstractSkill : IFSMState
{
    public abstract AssetLocation RegisterName { get; }
    public ISkillable User { get; }
    public FSMSystem System { get; }

    public AbstractSkill(ISkillable user)
    {
        User = user;
        System = user.SkillFsmSystem;
    }

    public abstract void Init();

    public abstract bool CanEnter();

    public abstract void OnEnter();

    public abstract void OnUpdate();

    public abstract void OnLeave();
}