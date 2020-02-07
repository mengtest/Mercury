using System;
using UnityEngine;

/// <summary>
/// 享元模式Dot buff实例
/// </summary>
public struct BuffFlyweightDot : IBuffFlyweight
{
    /// <summary>
    /// 原型
    /// </summary>
    public readonly DotBuff prototype;

    /// <summary>
    /// 来源
    /// </summary>
    public readonly Entity source;

    /// <summary>
    /// 下次触发时间
    /// </summary>
    public readonly float nextTime;

    /// <summary>
    /// 触发间隔
    /// </summary>
    public readonly float interval;

    /// <summary>
    /// 强度
    /// </summary>
    public readonly int intensity;

    /// <summary>
    /// 剩余触发次数，如果为-1则永久存在
    /// </summary>
    public int TriggerCount { get; set; }

    public AbstractBuff<T> GetPrototype<T>() where T : struct, IBuffFlyweight
    {
        var cast = prototype as AbstractBuff<T>;
        if (cast == null)
        {
            throw new InvalidCastException();
        }

        return cast;
    }

    /// <param name="prototype">原型</param>
    /// <param name="source">来源</param>
    /// <param name="interval">触发间隔</param>
    /// <param name="triggerCount">触发次数</param>
    /// <param name="intensity">强度</param>
    public BuffFlyweightDot(DotBuff prototype, Entity source, float interval, int triggerCount, int intensity) : this(
        prototype,
        source,
        interval,
        triggerCount,
        intensity,
        Time.time)
    {
    }

    /// <param name="prototype">原型</param>
    /// <param name="source">来源</param>
    /// <param name="duration">总持续时间</param>
    /// <param name="interval">触发间隔</param>
    /// <param name="intensity">强度</param>
    public BuffFlyweightDot(DotBuff prototype, Entity source, float duration, float interval, int intensity)
        : this(prototype, source, interval, (int) (duration / interval), intensity)
    {
    }

    public BuffFlyweightDot(
        DotBuff prototype,
        Entity source,
        float interval,
        int triggerCount,
        int intensity,
        float nextTime)
    {
        this.prototype = prototype;
        this.source = source;
        this.interval = interval;
        this.intensity = intensity;
        this.nextTime = nextTime;
        TriggerCount = triggerCount;
    }

    public BuffFlyweightDot AfterTrigger()
    {
        return new BuffFlyweightDot(prototype,
            source,
            interval,
            TriggerCount - 1,
            intensity,
            nextTime + interval);
    }
}