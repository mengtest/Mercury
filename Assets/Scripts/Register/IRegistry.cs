using System;
using System.Collections.Generic;

namespace Mercury
{
    public interface IRegistry
    {
        string RegistryName { get; }

        Type GetRegistryType();

        void PublishRegisterEvent(EventSystem eventSystem);
    }

    public interface IRegistry<T> : IRegistry where T : class, IRegistryEntry<T>
    {
        IReadOnlyDictionary<AssetLocation, IRegistryEntry<T>> Entries { get; }

        bool TryGetEntry(AssetLocation id, out T entry);

        bool ContainsEntry(AssetLocation id);

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

        public void Register(T entry)
        {
            if (ContainsEntry(entry.RegisterName))
            {
                throw new InvalidOperationException($"已注册过{entry.RegisterName}");
            }

            _entries.Add(entry.RegisterName, entry);
        }

        Type IRegistry.GetRegistryType() { return typeof(T); }

        void IRegistry.PublishRegisterEvent(EventSystem eventSystem)
        {
            eventSystem.Publish(this, new RegisterEvent<T>(this));
        }
    }
}