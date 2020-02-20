using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 风痕
/// </summary>
[AutoRegister]
public class BuffWindMark : AbstractBuff
{
    public override AssetLocation RegisterName { get; } = Consts.BuffWindMark;
    public override IReadOnlyList<AssetLocation> DependAssets { get; } = null;

    public override BuffStack Merge(BuffStack willAdded, BuffStack exist)
    {
        var intensity = exist.intensity;
        var nextTime = exist.nextTime;
        if (intensity < 10) //强度最大值是10
        {
            intensity += 1;
        }

        if (nextTime < willAdded.nextTime)//下次触发时间比较大的那个buff
        {
            nextTime = willAdded.nextTime;
        }

        return new BuffStack(exist.prototype, exist.source, exist.interval, 1, intensity, nextTime);
    }

    public override void OnFirstAdd(IBuffable holder, BuffStack buff)
    {
        //Debug.Log($"添加:{((Entity) holder).RegisterName}");
        (buff.source as EntityRaceter).HasWindMarkBuff.Add((Entity) holder);
    }

    public override void OnRepeatAdd(IBuffable holder, BuffStack buff) { }

    public override bool OnRemove(IBuffable holder, BuffStack buff)
    {
        //Debug.Log($"移除:{((Entity) holder).RegisterName}");
        return (buff.source as EntityRaceter).HasWindMarkBuff.Remove((Entity) holder);
    }

    public override void OnTrigger(IBuffable holder, BuffStack buff) { }
}