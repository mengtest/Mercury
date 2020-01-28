/// <summary>
/// 收益类型
/// </summary>
public enum DamageIncome
{
    /// <summary>
    /// 攻击力增加/减少
    /// </summary>
    Subjoin = 0,

    /// <summary>
    /// 伤害提升/减少
    /// </summary>
    Upgrade = 1,

    /// <summary>
    /// 默认，用于区分收益
    /// </summary>
    Default = -1
}