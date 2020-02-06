/// <summary>
/// 公共抽象Buff类
/// </summary>
/// <typeparam name="T">享元结构体</typeparam>
public abstract class AbstractBuff<T> where T : struct, IBuffFlyweight<T>
{
    /// <summary>
    /// 合并已有Buff和新添加Buff，在添加Buff时，如果已有Buff则会触发
    /// </summary>
    /// <returns>合并完成后的buff</returns>
    public abstract ref T Merge(ref T left, ref T right);

    /// <summary>
    /// 第一次添加Buff时调用
    /// </summary>
    /// <param name="holder">Buff持有者</param>
    /// <param name="buff">Buff享元实例</param>
    public abstract void OnFirstAdd(IBuffable holder, in T buff);

    /// <summary>
    /// 已有该Buff后再次添加，在调用Merge后调用
    /// </summary>
    /// <param name="holder">Buff持有者</param>
    /// <param name="buff">Buff享元实例</param>
    public abstract void OnRepeatAdd(IBuffable holder, in T buff);

    /// <summary>
    /// 删除Buff时调用
    /// </summary>
    /// <param name="holder">Buff持有者</param>
    /// <param name="buff">Buff享元实例</param>
    /// <returns>是否删除失败</returns>
    public abstract bool OnRemove(IBuffable holder, in T buff);
}