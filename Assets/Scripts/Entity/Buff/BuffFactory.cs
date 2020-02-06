using System;
using System.Collections.Generic;

public class BuffFactory : Singleton<BuffFactory>
{
    private readonly Dictionary<Type, DotBuff> _dots = new Dictionary<Type, DotBuff>();

    public IReadOnlyDictionary<Type, DotBuff> Dots => _dots;

    private BuffFactory() { }

    public void Register(DotBuff dot)
    {
        var type = dot.GetType();
        if (_dots.ContainsKey(type))
        {
            throw new InvalidOperationException();
        }

        _dots.Add(type, dot);
    }

    public BuffFlyweightDot GetDot<T>(Entity source, float interval, int triggerCount, int intensity) where T : DotBuff
    {
        return new BuffFlyweightDot(_dots[typeof(T)], source, interval, triggerCount, intensity);
    }
}