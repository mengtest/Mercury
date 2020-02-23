using System;

namespace Mercury
{
    public class RegisterEvent<T> : EventArgs where T : class, IRegistryEntry<T>
    {
        public IRegistry<T> Registry { get; }

        public RegisterEvent(IRegistry<T> registry) { Registry = registry; }
    }
}