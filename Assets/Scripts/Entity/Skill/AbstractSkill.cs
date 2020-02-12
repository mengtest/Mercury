/// <summary>
/// 抽象技能类
/// </summary>
public abstract class AbstractSkill : IFSMState
{
    /// <summary>
    /// 注册ID
    /// </summary>
    public abstract AssetLocation RegisterName { get; }

    /// <summary>
    /// 技能使用者
    /// </summary>
    public ISkillable User { get; }

    public FSMSystem System { get; }

    public AbstractSkill(ISkillable user)
    {
        User = user;
        System = user.SkillFsmSystem;
    }

    /// <summary>
    /// 初始化，在实例化对象后调用
    /// </summary>
    public abstract void Init();

    /// <summary>
    /// 是否可以使用该技能
    /// </summary>
    public abstract bool CanEnter();

    /// <summary>
    /// 当使用技能时触发
    /// </summary>
    public abstract void OnEnter();

    /// <summary>
    /// 技能使用中触发，每帧调用
    /// </summary>
    public abstract void OnUpdate();

    /// <summary>
    /// 技能出发完毕，离开时触发
    /// </summary>
    public abstract void OnLeave();
}