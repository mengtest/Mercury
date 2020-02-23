using System;
using System.Collections.Generic;

namespace Mercury
{
    public abstract class Entity
    {
        private readonly Dictionary<Type, IEntityComponent> _components;

        public AssetLocation RegisteredName { get; }

        protected Entity(AssetLocation id)
        {
            RegisteredName = id;
            _components = new Dictionary<Type, IEntityComponent>();
        }

        public bool GetComponent<T>(out T component)
        {
            if (!_components.TryGetValue(typeof(T), out var temp))
            {
                component = default;
                return false;
            }

            if (temp is T c)
            {
                component = c;
                return true;
            }

            component = default;
            return false;
        }

        public void AddComponent(IEntityComponent component) { _components.Add(component.GetType(), component); }
    }
}