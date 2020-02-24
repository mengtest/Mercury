using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mercury
{
    /// <summary>
    /// 注册管理
    /// </summary>
    public class RegisterManager
    {
        private readonly GameManager _gameManager;

        /// <summary>
        /// 注册表
        /// </summary>
        private readonly Dictionary<string, IRegistry> _registries;

        public RegisterManager(GameManager gameManager)
        {
            _gameManager = gameManager;
            _registries = new Dictionary<string, IRegistry>();
        }

        public void Init()
        {
            Entities.Init(this);
            Assets.Init(this);
            _gameManager.EventBus.Publish(this, new RegisterManagerInitEvent(this));
            EntryDependInfos.Init(this);
            CheckDependencies(EntryDependInfos.Registry);
            foreach (var registry in _registries.Values)
            {
                registry.PublishRegisterEvent(_gameManager.EventBus);
            }
        }

        private void CheckDependencies(IRegistry<EntryDependInfo> dependencies)
        {
            foreach (var dependency in dependencies.Entries)
            {
                var key = dependency.Key;
                var depend = (EntryDependInfo) dependency.Value;
                if (!ContainsRegistryEntry(depend.Key.entryType, depend.Key.id))
                {
                    Debug.LogError($"找不到依赖关系{key}的key{depend.Key.entryType}:{depend.Key.id}");
                }

                foreach (var dep in depend.Values)
                {
                    if (!ContainsRegistryEntry(dep.entryType, dep.id))
                    {
                        Debug.LogError($"找不到依赖关系{key}的value{dep.entryType}:{dep.id}");
                    }
                }
            }
        }

        /// <summary>
        /// 添加注册表
        /// </summary>
        /// <param name="registry">注册表实例</param>
        /// <exception cref="InvalidOperationException">重复添加注册表</exception>
        public void AddRegistry<T>(IRegistry<T> registry) where T : class, IRegistryEntry<T>
        {
            _gameManager.CheckState(GameState.Init, "只有Init状态可以添加注册表");
            if (_registries.ContainsKey(registry.RegistryName))
            {
                throw new InvalidOperationException($"已经添加过的注册表{registry.RegistryName}");
            }

            _registries.Add(registry.RegistryName, registry);
        }

        public T QueryRegistryEntry<T>(EntryLocation id) where T : class, IRegistryEntry<T> { return QueryRegistryEntry<T>(id.entryType, id.id); }

        /// <summary>
        /// 查询注册项
        /// </summary>
        /// <param name="type">注册表名</param>
        /// <param name="id">注册项id</param>
        /// <returns>查询到则返回注册项实例，否则返回null</returns>
        public T QueryRegistryEntry<T>(string type, AssetLocation id) where T : class, IRegistryEntry<T>
        {
            if (!QueryRegistry<T>(type, out var registry))
            {
                return null;
            }

            if (!registry.TryGetEntry(id, out var entry))
            {
                return null;
            }

            if (entry is T e)
            {
                return e;
            }

            throw new InvalidCastException($"类型错误:{entry.GetType()}尝试转换成{typeof(T)}");
        }

        public bool ContainsRegistryEntry(string type, AssetLocation id) { return QueryRegistry(type, out var r) && r.NoTypeEntries.ContainsKey(id); }

        /// <summary>
        /// 查询注册表
        /// </summary>
        /// <param name="type">注册表名</param>
        /// <param name="registry">注册表实例，未查询到返回null</param>
        /// <returns>是否查询到注册表</returns>
        public bool QueryRegistry<T>(string type, out IRegistry<T> registry) where T : class, IRegistryEntry<T>
        {
            if (_registries.TryGetValue(type, out var temp))
            {
                if (!(temp is IRegistry<T> e))
                {
                    throw new InvalidCastException($"类型错误:{temp.GetType()}尝试转换成{typeof(IRegistry<T>)}");
                }

                registry = e;
                return true;
            }

            registry = null;
            return false;
        }

        public bool QueryRegistry(string type, out IRegistry registry) { return _registries.TryGetValue(type, out registry); }
    }
}