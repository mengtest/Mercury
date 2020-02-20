using UnityEngine;

/// <summary>
/// Buff享元模式
/// </summary>
public sealed class BuffStack
{
    /// <summary>
    /// 原型
    /// </summary>
    public readonly AbstractBuff prototype;

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
    /// 触发次数
    /// </summary>
    public readonly int triggerCount;

    public BuffStack(
        AbstractBuff prototype,
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
        this.triggerCount = triggerCount;
        this.nextTime = nextTime;
    }

    public BuffStack(AbstractBuff prototype, Entity source, float interval, int triggerCount, int intensity) : this(
        prototype,
        source,
        interval,
        triggerCount,
        intensity,
        Time.time + interval)
    {
    }

    /// <summary>
    /// 获取触发完后的buff实例
    /// </summary>
    public BuffStack AfterTrigger()
    {
        return triggerCount < 0
            ? new BuffStack(prototype, source, interval, triggerCount, intensity, nextTime + interval)
            : new BuffStack(prototype, source, interval, triggerCount - 1, intensity, nextTime + interval);
    }

    public override string ToString()
    {
        return $"buff name:{prototype.RegisterName.ToString()} " +
               $"nextTime:{nextTime} " +
               $"interval:{interval} " +
               $"triggerCount:{triggerCount} " +
               $"intensity:{intensity}";
    }
}