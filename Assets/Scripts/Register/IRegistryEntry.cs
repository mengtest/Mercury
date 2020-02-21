using System.Collections.Generic;

public interface IRegistryEntry
{
    /// <summary>
    /// 注册名，全局唯一
    /// </summary>
    AssetLocation RegisterName { get; }

    /// <summary>
    /// 依赖的资源/注册项
    /// </summary>
    IReadOnlyList<AssetLocation> DependAssets { get; }
}