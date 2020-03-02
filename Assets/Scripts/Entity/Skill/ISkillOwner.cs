namespace Mercury
{
    /// <summary>
    /// 可以使用技能
    /// </summary>
    public interface ISkillOwner
    {
        /// <summary>
        /// 技能系统
        /// </summary>
        ISkillSystem SkillSystem { get; set; }
    }
}