using UnityEngine;

namespace Mercury
{
    public interface ISystemOwner
    {
        /// <summary>
        /// 添加系统
        /// </summary>
        void AddSystem<T>(T system) where T : MonoBehaviour, IEntitySystem;

        /// <summary>
        /// 获取系统
        /// </summary>
        T GetSystem<T>() where T : MonoBehaviour, IEntitySystem;
    }
}