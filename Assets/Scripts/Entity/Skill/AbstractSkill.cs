public abstract class AbstractSkill : IRegistryEntry
{
    public abstract AssetLocation RegisterName { get; }
    public abstract void Init(SkillStack stack);
    public abstract bool CanEnter(SkillStack stack);
    public abstract void OnEnter(SkillStack stack);
    public abstract void OnUpdate(SkillStack stack);
    public abstract void OnLeave(SkillStack stack);
}