public enum DamageType
{
    Physics = 0,
    Magic = 1,

    /// <summary>
    /// 需要注意的是，在计算伤害收益时该值表示全伤害而不是真伤
    /// </summary>
    True = 2
}