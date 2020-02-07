/// <summary>
/// 回血
/// </summary>
public class BuffHeal : DotBuff
{
    public override string Name { get; } = Consts.BUFF_Heal;

    public override ref BuffFlyweightDot Merge(ref BuffFlyweightDot willAdded, ref BuffFlyweightDot exist)
    {
        if (willAdded.intensity > exist.intensity)
        {
            return ref willAdded;
        }

        if (willAdded.intensity < exist.intensity)
        {
            return ref exist;
        }

        willAdded.TriggerCount += exist.TriggerCount;
        return ref willAdded;
    }

    public override void OnTrigger(IBuffable holder, in BuffFlyweightDot buff)
    {
        (holder as Entity)?.Heal(buff.intensity * 0.5f);
    }

    public override void OnFirstAdd(IBuffable holder, in BuffFlyweightDot buff) { }

    public override void OnRepeatAdd(IBuffable holder, in BuffFlyweightDot buff) { }

    public override bool OnRemove(IBuffable holder, in BuffFlyweightDot buff) { return true; }
}