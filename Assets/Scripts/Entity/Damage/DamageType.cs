/// <summary>
/// 伤害类型
/// </summary>
public enum DamageType
{
    /// <summary>
    /// 物理伤害
    /// </summary>
    Physics = 0,

    /// <summary>
    /// 魔法伤害
    /// </summary>
    Magic = 1,

    /// <summary>
    /// 真实伤害， 需要注意的是，在计算伤害收益时该值表示全伤害而不是真伤
    /// </summary>
    True = 2
}