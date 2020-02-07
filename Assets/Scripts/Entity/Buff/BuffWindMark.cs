public class BuffWindMark : StateBuff
{
    public override string Name { get; } = Consts.BUFF_WindMark;

    public override ref BuffFlyweightState Merge(ref BuffFlyweightState willAdded, ref BuffFlyweightState exist)
    {
        if (exist.intensity < 10)
        {
            exist.intensity += 1;
        }

        if (exist.ExpireTime < willAdded.ExpireTime)
        {
            exist.ExpireTime = willAdded.ExpireTime;
        }

        return ref exist;
    }

    public override void OnFirstAdd(IBuffable holder, in BuffFlyweightState buff)
    {
        (buff.source as EntityRaceter).HasWindMarkBuff.Add(holder as Entity);
    }

    public override void OnRepeatAdd(IBuffable holder, in BuffFlyweightState buff) { }

    public override bool OnRemove(IBuffable holder, in BuffFlyweightState buff)
    {
        return (buff.source as EntityRaceter).HasWindMarkBuff.Remove(holder as Entity);
    }
}