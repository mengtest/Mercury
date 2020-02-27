using System;
using System.Collections.Generic;

namespace Mercury
{
    [Flags]
    public enum EntityType
    {
        Player = 00,
        Enemy = 01,
        Neutral = 10
    }

    public abstract class Entity
    {
        /// <summary>
        /// 组件
        /// </summary>
        private readonly Dictionary<string, IEntityComponent> _components;

        /// <summary>
        /// 注册名
        /// </summary>
        public AssetLocation RegisteredName { get; }

        public EntityType Type { get; }

        protected Entity(AssetLocation id, EntityType type)
        {
            RegisteredName = id;
            Type = type;
            _components = new Dictionary<string, IEntityComponent>();
        }

        /// <summary>
        /// 获取一个实体组件
        /// </summary>
        /// <param name="id">组件id</param>
        /// <param name="component">返回组件实例</param>
        /// <typeparam name="T">组件类型</typeparam>
        /// <returns>是否拥有组件</returns>
        public bool GetComponent<T>(string id, out T component) where T : IEntityComponent
        {
            if (!_components.TryGetValue(id, out var temp))
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
        public void AddComponent(IEntityComponent component)
        {
            if (_components.ContainsKey(component.Id))
            {
                throw new ArgumentException($"已有组件id：{component.Id}");
            }

            _components.Add(component.Id, component);
        }

        public static void NonNullCheck(object obj, string message)
        {
            if (obj != null)
            {
                throw new InvalidOperationException(message);
            }
        }
    }
}