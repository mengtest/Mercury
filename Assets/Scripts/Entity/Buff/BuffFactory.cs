using System;
using System.Collections.Generic;

public class BuffFactory : Singleton<BuffFactory>
{
    private readonly Dictionary<string, DotBuff> _dots = new Dictionary<string, DotBuff>();
    private readonly Dictionary<string, StateBuff> _states = new Dictionary<string, StateBuff>();

    public IReadOnlyDictionary<string, DotBuff> Dots => _dots;
    public IReadOnlyDictionary<string, StateBuff> States => _states;

    private BuffFactory() { }

    /// <summary>
    /// 注册
    /// </summary>
    public void Register(DotBuff dot)
    {
        var type = dot.Name;
        if (_dots.ContainsKey(type))
        {
            throw new InvalidOperationException();
        }

        _dots.Add(type, dot);
    }

    /// <summary>
    /// 注册
    /// </summary>
    public void Register(StateBuff state)
    {
        var type = state.Name;
        if (_states.ContainsKey(type))
        {
            throw new InvalidOperationException();
        }

        _states.Add(type, state);
    }

    /// <summary>
    /// 构造Dot Buff实例
    /// </summary>
    /// <param name="name">Buff名</param>
    /// <param name="source">Buff来源</param>
    /// <param name="interval">持续时间</param>
    /// <param name="triggerCount">触发时间</param>
    /// <param name="intensity">强度</param>
    /// <returns>Buff实例</returns>
    public static BuffFlyweightDot GetDot(string name, Entity source, float interval, int triggerCount, int intensity)
    {
        return new BuffFlyweightDot(Instance._dots[name], source, interval, triggerCount, intensity);
    }

    /// <summary>
    /// 构造状态Buff实例
    /// </summary>
    /// <param name="stateName">状态名字</param>
    /// <param name="source">Buff来源</param>
    /// <param name="interval">持续时间</param>
    /// <param name="intensity">强度</param>
    /// <returns>Buff实例</returns>
    public static BuffFlyweightState GetState(string stateName, Entity source, float interval, int intensity)
    {
        return new BuffFlyweightState(Instance._states[stateName], source, interval, intensity);
    }
}