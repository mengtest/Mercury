using System;
using System.Collections.Generic;

/// <summary>
/// 抽象Buff类
/// </summary>
[EventSubscriber]
public abstract class AbstractBuff : IRegistryEntry
{
    public abstract AssetLocation RegisterName { get; }
    public abstract IReadOnlyList<AssetLocation> DependAssets { get; }

    /// <summary>
    /// 合并已有Buff和新添加Buff，在添加Buff时，如果已有Buff则会触发
    /// </summary>
    /// <param name="willAdded">将要添加的buff实例</param>
    /// <param name="exist">已存在的buff实例</param>
    /// <returns>合并完成后的buff</returns>
    public abstract BuffStack Merge(BuffStack willAdded, BuffStack exist);

    /// <summary>
    /// 第一次添加Buff时调用
    /// </summary>
    /// <param name="holder">Buff持有者</param>
    /// <param name="buff">Buff享元实例</param>
    public abstract void OnFirstAdd(IBuffable holder, BuffStack buff);

    /// <summary>
    /// 已有该Buff后再次添加，在调用Merge后调用
    /// </summary>
    /// <param name="holder">Buff持有者</param>
    /// <param name="buff">Buff享元实例</param>
    public abstract void OnRepeatAdd(IBuffable holder, BuffStack buff);

    /// <summary>
    /// 删除Buff时调用
    /// </summary>
    /// <param name="holder">Buff持有者</param>
    /// <param name="buff">Buff享元实例</param>
    /// <returns>是否删除失败</returns>
    public abstract bool OnRemove(IBuffable holder, BuffStack buff);

    /// <summary>
    /// 触发时调用
    /// </summary>
    /// <param name="holder">buff持有者</param>
    /// <param name="buff">享元</param>
    public abstract void OnTrigger(IBuffable holder, BuffStack buff);

    [Subscribe]
    private static void OnRegEve(object sender, RegisterEvent.Pre e) { e.manager.AddRegistryType(typeof(AbstractBuff), (type, _) => (IRegistryEntry) Activator.CreateInstance(type, true)); }
}