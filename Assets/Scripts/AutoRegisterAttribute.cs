using System;

/// <summary>
/// 自动注册特性
/// 目前仅支持实体(Entity)，技能(AbstractSkill)，Buff(AbstractBuff)的注册
/// 实体至少需要指定registerName
/// 技能至少需要指定registerName
/// Buff不需要指定任何参数
///
/// dependents中的技能支持自动添加
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class AutoRegisterAttribute : Attribute
{
    public readonly string registerName;
    public readonly string[] dependents;

    public AutoRegisterAttribute(string registerName = null, string[] dependents = null)
    {
        this.registerName = registerName;
        this.dependents = dependents;
    }
}