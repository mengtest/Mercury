using System;
using System.Collections.Generic;
using System.Linq;

namespace Mercury
{
    public interface IRegistry
    {
        /// <summary>
        /// 注册表名
        /// </summary>
        string RegistryName { get; }

        /// <summary>
        /// 注册表类型
        /// </summary>
        /// <returns></returns>
        Type GetRegistryType();

        /// <summary>
        /// 发布注册事件
        /// </summary>
        /// <param name="eventSystem">事件系统</param>
        void PublishRegisterEvent(EventSystem eventSystem);

        /// <summary>
        /// 无类型的注册表
        /// </summary>
        IReadOnlyDictionary<AssetLocation, IRegistryEntry> NoTypeEntries { get; }
    }

    public interface IRegistry<T> : IRegistry where T : class, IRegistryEntry<T>
    {
        /// <summary>
        /// 注册表
        /// </summary>
        IReadOnlyDictionary<AssetLocation, IRegistryEntry<T>> Entries { get; }

        /// <summary>
        /// 尝试获取
        /// </summary>
        /// <param name="id">元素id</param>
        /// <param name="entry">返回元素</param>
        bool TryGetEntry(AssetLocation id, out T entry);

        /// <summary>
        /// 是否已注册
        /// </summary>
        /// <param name="id">元素id</param>
        bool ContainsEntry(AssetLocation id);

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="entry">元素实例</param>
        void Register(T entry);
    }

    public class RegistryImpl<T> : IRegistry<T> where T : class, IRegistryEntry<T>
    {
        private readonly Dictionary<AssetLocation, IRegistryEntry<T>> _entries;

        public string RegistryName { get; }

        public IReadOnlyDictionary<AssetLocation, IRegistryEntry<T>> Entries => _entries;


        public RegistryImpl(string registryName)
        {
            RegistryName = registryName;
            _entries = new Dictionary<AssetLocation, IRegistryEntry<T>>();
        }

        public bool TryGetEntry(AssetLocation id, out T entry)
        {
            var res = _entries.TryGetValue(id, out var temp);
            if (res == false)
            {
                entry = default;
                return false;
            }

            if (!(temp is T type))
            {
                throw new ArgumentException($"类型错误,应该是{typeof(T).FullName},但获取的元素是{temp.GetType().FullName}");
            }

            entry = type;
            return true;
        }

        public bool ContainsEntry(AssetLocation id) { return _entries.ContainsKey(id); }

        public virtual void Register(T entry)
        {
            if (ContainsEntry(entry.RegisterName))
            {
                throw new InvalidOperationException($"已注册过{entry.RegisterName}");
            }

            _entries.Add(entry.RegisterName, entry);
        }

        Type IRegistry.GetRegistryType() { return typeof(T); }

        void IRegistry.PublishRegisterEvent(EventSystem eventSystem) { eventSystem.Publish(this, new RegisterEvent<T>(this)); }

        IReadOnlyDictionary<AssetLocation, IRegistryEntry> IRegistry.NoTypeEntries { get { return _entries.ToDictionary(e => e.Key, e => (IRegistryEntry) e.Value); } }
    }
}