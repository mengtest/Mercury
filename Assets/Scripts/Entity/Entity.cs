using System;
using System.Collections.Generic;

namespace Mercury
{
    public abstract class Entity
    {
        /// <summary>
        /// 组件
        /// </summary>
        private readonly Dictionary<Type, IEntityComponent> _components;

        /// <summary>
        /// 注册名
        /// </summary>
        public AssetLocation RegisteredName { get; }

        protected Entity(AssetLocation id)
        {
            RegisteredName = id;
            _components = new Dictionary<Type, IEntityComponent>();
        }

        /// <summary>
        /// 获取一个实体组件
        /// </summary>
        /// <param name="component">返回组件实例</param>
        /// <typeparam name="T">组件类型</typeparam>
        /// <returns>是否拥有组件</returns>
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

        /// <summary>
        /// 添加一个实体组件
        /// </summary>
        /// <param name="component">组件实例</param>
        public void AddComponent(IEntityComponent component) { _components.Add(component.GetType(), component); }
    }
}