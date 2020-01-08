using System;
using System.Linq;
using System.Reflection;

/// <summary>
/// 单例
/// </summary>
public abstract class Singleton<T> where T : class
{
    private static readonly Lazy<T> _instanceConstructor = new Lazy<T>(() =>
    {
        var type = typeof(T);
        if (type.IsAbstract)
        {
            throw new ArgumentException($"单例类{type.FullName}不能是抽象的");
        }

        var constructors =
            type.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        if (constructors.Length != 1)
        {
            throw new ArgumentException($"单例类{type.FullName}不能有多个构造方法");
        }

        var constructor = constructors.SingleOrDefault(c => !c.GetParameters().Any() && c.IsPrivate);
        if (constructor == null)
        {
            throw new ArgumentException($"单例类{type.FullName}的构造方法必须是私有且无参的");
        }

        return Activator.CreateInstance(type, true) as T;
    }, true);

    /// <summary>
    /// 获取单例实例
    /// </summary>
    public static T Instance => _instanceConstructor.Value;
}
