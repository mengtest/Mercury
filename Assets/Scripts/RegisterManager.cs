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
        _matchChain = new List<Func<Type, Type>>
        {
            type => type.IsSubclassOf(typeof(AbstractBuff)) ? typeof(AbstractBuff) : null,
            type => type.IsSubclassOf(typeof(Entity)) ? typeof(Entity) : null,
            type => type.IsSubclassOf(typeof(AbstractSkill)) ? typeof(AbstractSkill) : null,
        };
        _regFunc = new Dictionary<Type, Func<Type, Attribute, IRegistryEntry>>
        {
            {typeof(AbstractBuff), (type, _) => (IRegistryEntry) Activator.CreateInstance(type, true)},
            {
                typeof(Entity), (type, attr) =>
                {
                    var autoReg = (AutoRegisterAttribute) attr;
                    var builder = EntityEntry.Create()
                        .SetRegisterName(new AssetLocation(Consts.Mercury, Consts.Entity, autoReg.registerName));
                    if (autoReg.dependents == null)
                    {
                        return builder.Build();
                    }

                    foreach (var dependent in autoReg.dependents)
                    {
                        var group = dependent.Split('.');
                        if (group.Length != 2)
                        {
                            Debug.LogError($"字符串{dependent}解析失败,略过");
                        }

                        builder.AddDependEntry(new AssetLocation(Consts.Mercury, group[0], group[1]));
                    }

                    return builder.Build();
                }
            },
            {
                typeof(AbstractSkill), (type, attr) =>
                {
                    var autoReg = (AutoRegisterAttribute) attr;
                    var builder = SkillEntry.Create()
                        .SetRegisterName(new AssetLocation(Consts.Mercury, Consts.Skill, autoReg.registerName))
                        .SetSkillType(type);
                    if (autoReg.dependents == null)
                    {
                        return builder.Build();
                    }

                    foreach (var dependent in autoReg.dependents)
                    {
                        var group = dependent.Split('.');
                        if (@group.Length != 2)
                        {
                            Debug.LogError($"字符串{dependent}解析失败,略过");
                        }

                        builder.AddDependAsset(new AssetLocation(Consts.Mercury, @group[0], @group[1]));
                    }

                    return builder.Build();
                }
            }
        };
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
}