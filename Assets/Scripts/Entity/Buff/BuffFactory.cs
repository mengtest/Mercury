using System;
using System.Collections.Generic;

public class BuffFactory : Singleton<BuffFactory>
{
    private readonly Dictionary<Type, DotBuff> _dots = new Dictionary<Type, DotBuff>();
    private readonly Dictionary<Type, StateBuff> _states = new Dictionary<Type, StateBuff>();

    public IReadOnlyDictionary<Type, DotBuff> Dots => _dots;
    public IReadOnlyDictionary<Type, StateBuff> States => _states;

    private BuffFactory() { }

    /// <summary>
    /// 注册
    /// </summary>
    public void Register(DotBuff dot) { Register(dot, _dots); }

    /// <summary>
    /// 注册
    /// </summary>
    public void Register(StateBuff state) { Register(state, _states); }

    private static void Register<T>(T buff, IDictionary<Type, T> dict)
    {
        var type = buff.GetType();
        if (dict.ContainsKey(type))
        {
            throw new InvalidOperationException();
        }

        dict.Add(type, buff);
    }

    /// <summary>
    /// 构造Dot Buff实例
    /// </summary>
    /// <param name="source">Buff来源</param>
    /// <param name="interval">持续时间</param>
    /// <param name="triggerCount">触发时间</param>
    /// <param name="intensity">强度</param>
    /// <typeparam name="T">Buff类型</typeparam>
    /// <returns>Buff实例</returns>
    public BuffFlyweightDot GetDot<T>(Entity source, float interval, int triggerCount, int intensity) where T : DotBuff
    {
        return new BuffFlyweightDot(_dots[typeof(T)], source, interval, triggerCount, intensity);
    }
}