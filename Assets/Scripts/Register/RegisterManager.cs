using System;
using System.Collections.Generic;

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
            _gameManager.EventBus.Publish(this, new RegisterManagerInitEvent(this));
            foreach (var registry in _registries.Values)
            {
                registry.PublishRegisterEvent(_gameManager.EventBus);
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
    }
}