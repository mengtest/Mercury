public abstract class DotBuff
{
    public abstract BuffFlyweightDot Merge(ref BuffFlyweightDot left, ref BuffFlyweightDot right);

    public abstract bool IsReady(ref BuffFlyweightDot buff);

    public abstract void OnTrigger(ref BuffFlyweightDot buff);

    public abstract void OnFirstAdd(ref BuffFlyweightDot buff);

    public abstract void OnRepeatAdd(ref BuffFlyweightDot buff);

    public abstract bool OnRemove(ref BuffFlyweightDot buff);
}