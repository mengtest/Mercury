public abstract class DotBuff
{
    public abstract ref BuffFlyweightDot Merge(ref BuffFlyweightDot left, ref BuffFlyweightDot right);

    public abstract void OnTrigger(IBuffable holder, in BuffFlyweightDot buff);

    public abstract void OnFirstAdd(IBuffable holder, in BuffFlyweightDot buff);

    public abstract void OnRepeatAdd(IBuffable holder, in BuffFlyweightDot buff);

    public abstract bool OnRemove(IBuffable holder, in BuffFlyweightDot buff);
}