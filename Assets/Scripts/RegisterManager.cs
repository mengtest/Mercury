using System;
using System.Collections.Generic;

public sealed class RegisterManager : Singleton<RegisterManager>
{
    private readonly Dictionary<AssetLocation, IRegistryEntry> _entries;

    public IReadOnlyDictionary<AssetLocation, IRegistryEntry> Entries => _entries;

    private RegisterManager() { _entries = new Dictionary<AssetLocation, IRegistryEntry>(); }

    public static void Register(IRegistryEntry entry)
    {
        if (Instance._entries.ContainsKey(entry.RegisterName))
        {
            throw new ArgumentException();
        }

        Instance._entries.Add(entry.RegisterName, entry);
    }

    public static void OnEntityAwake(AssetLocation assetLocation, Entity entity)
    {
        if (!Instance._entries.TryGetValue(assetLocation, out var registryEntry))
        {
            throw new ArgumentException($"未注册:{assetLocation.ToString()}");
        }

        if (!(registryEntry is EntityEntry entityEntry))
        {
            throw new ArgumentException();
        }

        entityEntry.OnEntityAwake?.Invoke(entity);
    }

    public static void OnEntityStart(AssetLocation assetLocation, Entity entity)
    {
        if (!Instance._entries.TryGetValue(assetLocation, out var registryEntry))
        {
            throw new ArgumentException($"未注册:{assetLocation.ToString()}");
        }

        if (!(registryEntry is EntityEntry entityEntry))
        {
            throw new ArgumentException();
        }

        entityEntry.OnEntityStart?.Invoke(entity);
    }
}