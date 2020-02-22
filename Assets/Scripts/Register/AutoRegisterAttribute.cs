using System;

/// <summary>
/// 自动注册特性
/// 目前仅Buff(AbstractBuff)的注册
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
    [Obsolete]
    public class IdAttribute : Attribute
    {
    }

    /// <summary>
    /// 拥有该特性的方法只能是无参，静态，返回值为AssetLocation[]的方法
    /// 没有依赖的资源可以不使用该特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    [Obsolete]
    public class DependAttribute : Attribute
    {
    }
}