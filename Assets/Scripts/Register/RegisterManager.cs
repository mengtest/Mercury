using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public sealed class RegisterManager : Singleton<RegisterManager>
{
    private readonly Dictionary<AssetLocation, IRegistryEntry> _entries;
    private List<Func<Type, Type>> _matchChain;
    private Dictionary<Type, Func<Type, Attribute, IRegistryEntry>> _regFunc;

    public bool IsActive { get; private set; }
    public IReadOnlyDictionary<AssetLocation, IRegistryEntry> Entries => _entries;

    private RegisterManager()
    {
        _entries = new Dictionary<AssetLocation, IRegistryEntry>();
        _matchChain = new List<Func<Type, Type>>(4);
        _regFunc = new Dictionary<Type, Func<Type, Attribute, IRegistryEntry>>(4);
        IsActive = true;
    }

    public void Init()
    {
        EventManager.Instance.Publish(this, new RegisterEvent.Pre(Instance));
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
                foreach (var chain in Instance._matchChain)
                {
                    var res = chain(type);
                    if (res != null)
                    {
                        regState = true;
                        Register(Instance._regFunc[res](type, autoReg));
                        break;
                    }
                }

                if (!regState)
                {
                    Debug.LogError($"不支持的类型{type.FullName},略过");
                }
            }
        }

        Instance._matchChain = null;
        Instance._regFunc = null;
        EventManager.Instance.Publish(this, new RegisterEvent.AfterAuto(Instance));
        EventManager.Instance.Unsubscribe<RegisterEvent.Pre>();
        EventManager.Instance.Unsubscribe<RegisterEvent.AfterAuto>();
        IsActive = false;
    }

    public void Register(IRegistryEntry entry)
    {
        CheckState();
        if (_entries.ContainsKey(entry.RegisterName))
        {
            throw new ArgumentException();
        }

        _entries.Add(entry.RegisterName, entry);
    }

    public void AddRegistryType(Type baseType, Func<Type, Attribute, IRegistryEntry> regFunc)
    {
        CheckState();
        Instance._matchChain.Add(t => t.IsSubclassOf(baseType) ? baseType : null);
        Instance._regFunc.Add(baseType, regFunc);
    }

    private void CheckState()
    {
        if (!IsActive)
        {
            throw new InvalidOperationException($"当前状态无法注册");
        }
    }
}