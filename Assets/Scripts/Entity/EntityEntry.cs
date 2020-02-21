using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

[EventSubscriber]
public class EntityEntry : IRegistryEntry
{
    /// <summary>
    /// 实体的注册名用于寻找对应预制体，所以一个注册名对应一个预制体
    /// </summary>
    public AssetLocation RegisterName { get; }

    /// <summary>
    /// 实体的依赖资源表示：注册项，例如的技能、Buff之类，而不是资源
    /// </summary>
    public IReadOnlyList<AssetLocation> DependAssets { get; }

    public EntityEntry(
        AssetLocation registerName,
        IReadOnlyList<AssetLocation> dependAssets = null)
    {
        RegisterName = registerName;
        DependAssets = dependAssets;
    }

    public static Builder Create() { return new Builder(); }

    [Subscribe]
    private static void OnRegisterManagerPre(object sender, RegisterEvent.Pre e) { e.manager.AddRegistryType(typeof(Entity), AutoRegisterFunc); }

    private static bool SelectMethod(Type attr, MethodInfo m, Type type, Type target)
    {
        var a = m.GetCustomAttribute(attr);
        if (a == null)
        {
            return false;
        }

        if (m.GetParameters().Length != 0)
        {
            Debug.LogError($"{type.FullName}:{m.Name},获取id的方法不能有参数");
            return false;
        }

        if (m.ReturnType == target)
        {
            return true;
        }

        Debug.LogError($"{type.FullName}:{m.Name},获取id的方法返回值不是{typeof(AssetLocation)}");
        return false;
    }

    private static EntityEntry AutoRegisterFunc(Type type, Attribute attr)
    {
        var methods = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        var name = methods
            .Where(m => SelectMethod(typeof(AutoRegisterAttribute.IdAttribute), m, type, typeof(AssetLocation)))
            .ToArray();
        var dep = methods
            .Where(m => SelectMethod(typeof(AutoRegisterAttribute.DependAttribute), m, type, typeof(AssetLocation[])))
            .ToArray();
        if (name.Length != 1)
        {
            throw new ArgumentException($"{type.FullName}必须且只能有一个静态方法拥有特性{typeof(AutoRegisterAttribute.IdAttribute)}");
        }

        if (dep.Length > 1)
        {
            throw new ArgumentException($"{type.FullName}最多能有一个静态方法拥有特性{typeof(AutoRegisterAttribute.DependAttribute)}");
        }

        var id = (AssetLocation) name[0].Invoke(null, null);
        var depend = dep.Length == 1 ? dep[0].Invoke(null, null) : null;
        var builder = Create().SetRegisterName(id);
        if (depend == null)
        {
            return builder.Build();
        }

        var resType = (AssetLocation[]) depend;
        foreach (var res in resType)
        {
            builder.AddDependEntry(res);
        }

        return builder.Build();
    }

    public class Builder
    {
        private AssetLocation _registerName;
        private readonly List<AssetLocation> _dependRegistryEntries;

        internal Builder() { _dependRegistryEntries = new List<AssetLocation>(); }

        public Builder SetRegisterName(in AssetLocation registerName)
        {
            _registerName = registerName;
            return this;
        }

        public Builder AddDependEntry(in AssetLocation registerName)
        {
            _dependRegistryEntries.Add(registerName);
            return this;
        }

        public EntityEntry Build()
        {
            if (_registerName == null)
            {
                throw new ArgumentException();
            }

            if (_dependRegistryEntries.Count == 0)
            {
                return new EntityEntry(_registerName);
            }

            var skillList = _dependRegistryEntries
                .Where(entry => entry.type == Consts.Skill)
                .ToArray();
            EventManager.Instance.Subscribe((object sender, EntityEvent.Start e) =>
            {
                // if (!e.entity.RegisterName.Equals(_registerName))
                // {
                //     return;
                // }

                if (!(e.entity is ISkillable s))
                {
                    return;
                }

                foreach (var skill in skillList)
                {
                    s.AddSkill(EntityUtility.GetSkill(skill, s));
                }
            });
            return new EntityEntry(_registerName, _dependRegistryEntries.ToArray());
        }
    }
}