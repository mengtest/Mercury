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

    public static void OnEntityInstantiate(AssetLocation assetLocation)
    {
        if (!Instance._entries.ContainsKey(assetLocation))
        {
            throw new ArgumentException($"未注册:{assetLocation.ToString()}");
        }
    }
}