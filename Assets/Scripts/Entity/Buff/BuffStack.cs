using UnityEngine;

/// <summary>
/// Buff享元模式
/// </summary>
public class BuffStack
{
    public readonly AbstractBuff prototype;
    public readonly Entity source;
    public readonly float nextTime;
    public readonly float interval;
    public readonly int intensity;
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

    public BuffStack AfterTrigger() { return new BuffStack(prototype, source, interval, triggerCount - 1, intensity); }

    public override string ToString()
    {
        return $"buff name:{prototype.RegisterName.ToString()} " +
               $"nextTime:{nextTime} " +
               $"interval:{interval} " +
               $"triggerCount:{triggerCount} " +
               $"intensity:{intensity}";
    }
}