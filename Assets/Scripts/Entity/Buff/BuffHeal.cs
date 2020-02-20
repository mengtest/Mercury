using System.Collections.Generic;

/// <summary>
/// 回血
/// </summary>
[AutoRegister]
public class BuffHeal : AbstractBuff
{
    public override AssetLocation RegisterName { get; } = Consts.BuffHeal;
    public override IReadOnlyList<AssetLocation> DependAssets { get; } = null;

    public override BuffStack Merge(BuffStack willAdded, BuffStack exist)
    {
        if (willAdded.intensity > exist.intensity) //返回强度大的
        {
            return willAdded;
        }

        if (willAdded.intensity < exist.intensity)
        {
            return exist;
        }


        return new BuffStack(willAdded.prototype,
            willAdded.source,
            willAdded.interval,
            willAdded.triggerCount + exist.triggerCount,
            willAdded.intensity); //强度相同的话返回它们触发次数相加
    }

    public override void OnTrigger(IBuffable holder, BuffStack buff) { (holder as Entity)?.Heal(buff.intensity * 0.5f); }

    public override void OnFirstAdd(IBuffable holder, BuffStack buff) { }

    public override void OnRepeatAdd(IBuffable holder, BuffStack buff) { }

    public override bool OnRemove(IBuffable holder, BuffStack buff) { return true; }
}