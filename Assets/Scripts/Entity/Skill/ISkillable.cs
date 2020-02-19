using UnityEngine;

public interface ISkillable
{
    /// <summary>
    /// 技能状态机
    /// </summary>
    FSMSystem SkillFsmSystem { get; }

    /// <summary>
    /// 技能集合
    /// </summary>
    GameObject SkillCollection { get; }

    /// <summary>
    /// 获取属性
    /// </summary>
    T GetProperty<T>() where T : class, IEntityProperty;

    /// <summary>
    /// 添加技能
    /// </summary>
    void AddSkill(IFSMState skill);

    /// <summary>
    /// 移除技能
    /// </summary>
    bool RemoveSkill(AssetLocation location);

    /// <summary>
    /// 使用技能
    /// </summary>
    /// <param name="location">技能ID</param>
    void UseSkill(AssetLocation location);

    /// <summary>
    /// 使用技能
    /// </summary>
    /// <param name="location">技能ID</param>
    /// <param name="skill">技能实例</param>
    void UseSkill(AssetLocation location, out IFSMState skill);

    /// <summary>
    /// 每帧调用
    /// </summary>
    void OnUpdateSkills();
}