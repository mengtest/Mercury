using System;
using System.Collections.Generic;

public class BuffWindPace : AbstractBuff
{
    public override AssetLocation RegisterName { get; } = Consts.BuffWindPace;

    public override IReadOnlyList<AssetLocation> DependAssets { get; } = null;

    public override BuffStack Merge(BuffStack willAdded, BuffStack exist) { return exist; }

    public override void OnFirstAdd(IBuffable holder, BuffStack buff)
    {
        if (!(holder is EntityRaceter raceter))
        {
            throw new InvalidOperationException();
        }

        const float i = 0.3f;
        raceter.MotionCalculator.Add(i, MotionDataType.Move);
        raceter.MotionCalculator.Add(i, MotionDataType.Jump);
    }

    public override void OnRepeatAdd(IBuffable holder, BuffStack buff) { throw new InvalidOperationException(); }

    public override bool OnRemove(IBuffable holder, BuffStack buff)
    {
        if (!(holder is EntityRaceter raceter))
        {
            throw new InvalidOperationException();
        }

        const float i = 0.3f;
        var res = raceter.MotionCalculator.Remove(i, MotionDataType.Move) & raceter.MotionCalculator.Remove(i, MotionDataType.Jump);
        //Debug.Log($"rm res:{res}");
        return res;
    }

    public override void OnTrigger(IBuffable holder, BuffStack buff)
    {
        if (!(holder is EntityRaceter raceter))
        {
            throw new InvalidOperationException();
        }

        //Debug.Log($"{buff.intensity} {buff.interval} {buff.triggerCount} {buff.nextTime}");
        var sr = raceter.GetProperty<SwordResolve>();
        sr.OverlayResolve(-1 * buff.intensity);
    }
}