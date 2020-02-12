using System.Collections.Generic;

public class BuffWindMark : AbstractBuff
{
    public override AssetLocation RegisterName { get; } = Consts.BuffWindMark;
    public override IReadOnlyList<AssetLocation> DependAssets { get; } = null;

    public override BuffStack Merge(BuffStack willAdded, BuffStack exist)
    {
        var intensity = exist.intensity;
        var nextTime = exist.nextTime;
        if (intensity < 10)
        {
            intensity += 1;
        }

        if (nextTime < willAdded.nextTime)
        {
            nextTime = willAdded.nextTime;
        }

        return new BuffStack(exist.prototype, exist.source, exist.interval, 1, intensity, nextTime);
    }

    public override void OnFirstAdd(IBuffable holder, BuffStack buff)
    {
        (buff.source as EntityRaceter).HasWindMarkBuff.Add(holder as Entity);
    }

    public override void OnRepeatAdd(IBuffable holder, BuffStack buff) { }

    public override bool OnRemove(IBuffable holder, BuffStack buff)
    {
        return (buff.source as EntityRaceter).HasWindMarkBuff.Remove(holder as Entity);
    }

    public override void OnTrigger(IBuffable holder, BuffStack buff) { }
}