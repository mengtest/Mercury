public class BuffFlag : StateBuff
{
    public override string Name { get; }

    public BuffFlag(string name) { Name = name; }

    public override ref BuffFlyweightState Merge(ref BuffFlyweightState willAdded, ref BuffFlyweightState exist)
    {
        willAdded.ExpireTime += exist.interval;
        return ref willAdded;
    }

    public override void OnFirstAdd(IBuffable holder, in BuffFlyweightState buff) { }

    public override void OnRepeatAdd(IBuffable holder, in BuffFlyweightState buff) { }

    public override bool OnRemove(IBuffable holder, in BuffFlyweightState buff) { return true; }
}