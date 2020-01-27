/// <summary>
/// 伤害收益类型
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

public enum DamageUpgradeType
{
    None = -1,
    Physics = 0,
    Magic = 1,
    All = 2
}