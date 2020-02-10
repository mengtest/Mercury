using System.Collections.Generic;

public interface IRegistryEntry
{
    AssetLocation RegisterName { get; }

    IReadOnlyList<AssetLocation> DependAssets { get; }
}