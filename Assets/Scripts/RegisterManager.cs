using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public sealed class RegisterManager : Singleton<RegisterManager>
{
    private readonly Dictionary<AssetLocation, IRegistryEntry> _entries;
    private List<Func<Type, Type>> _matchChain;
    private Dictionary<Type, Func<Type, Attribute, IRegistryEntry>> _regFunc;

    public IReadOnlyDictionary<AssetLocation, IRegistryEntry> Entries => _entries;

    private RegisterManager()
    {
        _entries = new Dictionary<AssetLocation, IRegistryEntry>();
        _matchChain = new List<Func<Type, Type>>(4);
        _regFunc = new Dictionary<Type, Func<Type, Attribute, IRegistryEntry>>(4);
    }

    public static void Init()
    {
        var asm = typeof(RegisterManager).Assembly;
        foreach (var type in asm.ExportedTypes)
        {
            if (type.IsAbstract)
            {
                continue;
            }

            if (type.IsInterface)
            {
                continue;
            }

            if (type.IsValueType)
            {
                continue;
            }

            var attr = type.GetCustomAttributes();
            foreach (var attribute in attr)
            {
                if (!(attribute is AutoRegisterAttribute autoReg))
                {
                    continue;
                }

                var regState = false;
                foreach (var res in Instance._matchChain.Select(chain => chain(type)).Where(res => res != null))
                {
                    regState = true;
                    Register(Instance._regFunc[res](type, autoReg));
                    break;
                }

                if (!regState)
                {
                    Debug.LogError($"不支持的类型{type.FullName},略过");
                }
            }
        }

        Instance._matchChain = null;
        Instance._regFunc = null;
    }

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

    public static void AddRegistryType(Type baseType, Func<Type, Attribute, IRegistryEntry> regFunc)
    {
        Instance._matchChain.Add(t => t.IsSubclassOf(baseType) ? baseType : null);
        Instance._regFunc.Add(baseType, regFunc);
    }
}