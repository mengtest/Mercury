using System.Collections.Generic;

public class EntityEntry : IRegistryEntry
{
    public AssetLocation RegisterName { get; }
    public IReadOnlyList<AssetLocation> DependAssets { get; }

    public EntityEntry(AssetLocation registerName, IReadOnlyList<AssetLocation> dependAssets = null)
    {
        RegisterName = registerName;
        DependAssets = dependAssets;
    }
}