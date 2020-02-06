/// <summary>
/// 回血
/// </summary>
public class BuffHeal : DotBuff
{
    public override ref BuffFlyweightDot Merge(ref BuffFlyweightDot left, ref BuffFlyweightDot right)
    {
        if (left.intensity > right.intensity)
        {
            return ref left;
        }

        if (left.intensity < right.intensity)
        {
            return ref right;
        }

        left.TriggerCount += right.TriggerCount;
        return ref left;
    }

    public override void OnTrigger(IBuffable holder, in BuffFlyweightDot buff)
    {
        (holder as Entity)?.Heal(buff.intensity * 0.5f);
    }

    public override void OnFirstAdd(IBuffable holder, in BuffFlyweightDot buff) { }

    public override void OnRepeatAdd(IBuffable holder, in BuffFlyweightDot buff) { }

    public override bool OnRemove(IBuffable holder, in BuffFlyweightDot buff) { return true; }
}