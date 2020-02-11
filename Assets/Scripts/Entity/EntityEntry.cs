using System;
using System.Collections.Generic;

public class EntityEntry : IRegistryEntry
{
    public AssetLocation RegisterName { get; }
    public IReadOnlyList<AssetLocation> DependAssets { get; }
    public Action<Entity> OnEntityAwake { get; }
    public Action<Entity> OnEntityStart { get; }

    public EntityEntry(
        AssetLocation registerName,
        IReadOnlyList<AssetLocation> dependAssets = null,
        Action<Entity> onAwake = null,
        Action<Entity> onStart = null)
    {
        RegisterName = registerName;
        DependAssets = dependAssets;
        OnEntityAwake = onAwake;
        OnEntityStart = onStart;
    }
}