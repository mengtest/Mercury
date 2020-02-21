using System;

/// <summary>
/// 自动注册特性
/// 目前仅支持实体(Entity)，技能(AbstractSkill)，Buff(AbstractBuff)的注册
/// 实体至少需要指定AutoRegisterAttribute.Id特性
/// 技能至少需要指定AutoRegisterAttribute.Id特性
/// Buff不需要指定任何参数
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class AutoRegisterAttribute : Attribute
{
    public readonly string registerName;
    public readonly string[] dependents;

    public AutoRegisterAttribute() { }

    [Obsolete("请使用AutoRegisterAttribute.Id和AutoRegisterAttribute.Depend")]
    public AutoRegisterAttribute(string registerName, params string[] dependents)
    {
        this.registerName = registerName;
        this.dependents = dependents.Length != 0 ? dependents : null;
    }

    [Obsolete("请使用AutoRegisterAttribute.Id和AutoRegisterAttribute.Depend")]
    public AutoRegisterAttribute(string registerName) { this.registerName = registerName; }

    /// <summary>
    /// 拥有该特性的方法只能是无参，静态，返回值为AssetLocation的方法
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class IdAttribute : Attribute
    {
    }

    /// <summary>
    /// 拥有该特性的方法只能是无参，静态，返回值为AssetLocation[]的方法
    /// 没有依赖的资源可以不使用该特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class DependAttribute : Attribute
    {
    }
}