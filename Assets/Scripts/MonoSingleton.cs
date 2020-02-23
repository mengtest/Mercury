using UnityEngine;

namespace Mercury
{
    /// <summary>
    /// 可挂载单例
    /// </summary>
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        /// <summary>
        /// 获取单例实例
        /// </summary>
        public static T Instance
        {
            get
            {
                if (_instance) //已经获取过实例
                {
                    return _instance;
                }

                _instance = FindObjectOfType<T>(); //寻找场景中的实例
                if (_instance)
                {
                    return _instance;
                }

                var obj = new GameObject {name = typeof(T).Name}; //场景中不存在，实例化一个GO
                _instance = obj.AddComponent<T>();
                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if (_instance)
            {
                return;
            }

            _instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
    }
}