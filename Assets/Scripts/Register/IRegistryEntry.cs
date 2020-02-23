using System;

namespace Mercury
{
    public interface IRegistryEntry<out T> where T : class, IRegistryEntry<T>
    {
        AssetLocation RegisterName { get; }

        T SetRegisterName(AssetLocation id);

        Type GetRegisterType();
    }

    public class RegistryEntryImpl<T> : IRegistryEntry<T> where T : class, IRegistryEntry<T>
    {
        private AssetLocation _registerName;

        public AssetLocation RegisterName => _registerName;

        public T SetRegisterName(AssetLocation id)
        {
            if (_registerName != null)
            {
                throw new InvalidOperationException($"已经设置过注册名{_registerName}，不可重复设置");
            }

            _registerName = id;
            return this as T;
        }

        Type IRegistryEntry<T>.GetRegisterType() { return typeof(T); }
    }
}