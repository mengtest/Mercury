public interface IBuffable
{
    /// <summary>
    /// 每帧调用
    /// </summary>
    void OnUpdateBuffs();

    /// <summary>
    /// 添加Buff
    /// </summary>
    /// <param name="buff">Buff享元实例</param>
    void AddBuff(BuffStack buff);

    /// <summary>
    /// 移除Buff
    /// </summary>
    /// <param name="location">Buff ID</param>
    /// <returns>是否成功移除</returns>
    bool RemoveBuff(AssetLocation location);

    /// <summary>
    /// 是否拥有Buff
    /// </summary>
    /// <param name="location">Buff ID</param>
    bool HasBuff(AssetLocation location);

    /// <summary>
    /// 直接获取拥有的Buff
    /// </summary>
    /// <param name="location">Buff ID</param>
    BuffStack GetBuff(AssetLocation location);

    /// <summary>
    /// 尝试获取Buff
    /// </summary>
    /// <param name="location">Buff ID</param>
    /// <param name="buff">Buff享元实例</param>
    /// <returns>是否获取成功</returns>
    bool TryGetBuff(AssetLocation location, out BuffStack buff);
}