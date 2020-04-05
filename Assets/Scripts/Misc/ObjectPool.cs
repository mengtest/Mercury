using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mercury
{
    /// <summary>
    /// TODO:似乎不需要对象追踪
    /// </summary>
    public class ObjectPool
    {
        private readonly List<(GameObject, Stack<GameObject>)> _pool;
        private readonly bool _isTrace;
        private readonly Dictionary<GameObject, int> _active;

        /// <param name="isTrace">是否跟踪被获取的对象</param>
        public ObjectPool(bool isTrace)
        {
            _pool = new List<(GameObject, Stack<GameObject>)>();
            _isTrace = isTrace;
            if (_isTrace)
            {
                _active = new Dictionary<GameObject, int>();
            }
        }

        /// <summary>
        /// 添加可池化的预制体
        /// </summary>
        /// <param name="prefab">预制体</param>
        /// <param name="initCount">初始化数量</param>
        /// <returns>唯一id</returns>
        public int Allocate(GameObject prefab, int initCount)
        {
            var res = _pool.Find(s => s.Item1.Equals(prefab));
            if (res != default)
            {
                throw new ArgumentException($"已经添加过的预制体:{prefab.name}");
            }

            var index = _pool.Count;
            var stack = new Stack<GameObject>(initCount);
            for (var i = 0; i < initCount; i++)
            {
                stack.Push(Instantiate(prefab));
            }

            _pool.Add((prefab, stack));
            return index;
        }

        /// <summary>
        /// 获取一个实例
        /// </summary>
        /// <param name="id">唯一id</param>
        /// <returns>实例</returns>
        /// <exception cref="ArgumentException">id不存在</exception>
        public GameObject Get(int id)
        {
            IdCheck(id);
            var (prefab, stack) = _pool[id];
            var res = stack.Count > 0 ? stack.Pop() : Instantiate(prefab);
            if (_isTrace)
            {
                _active.Add(res, id);
            }

            return res;
        }

        /// <summary>
        /// 回收对象
        /// </summary>
        /// <param name="gameObject">需要回收的对象</param>
        /// <exception cref="ArgumentException">未启用对象追踪却调用该方法</exception>
        public void Recycle(GameObject gameObject)
        {
            if (!_isTrace)
            {
                throw new ArgumentException($"未追踪对象，请手动指定id");
            }

            if (_active.TryGetValue(gameObject, out var id))
            {
                _pool[id].Item2.Push(gameObject);
                _active.Remove(gameObject);
            }
            else
            {
                throw new ArgumentException($"GameObject:{gameObject.name} 不是从本池分配出去的");
            }
        }

        /// <summary>
        /// 回收对象
        /// </summary>
        /// <param name="gameObject">需要回收的对象</param>
        /// <param name="id">唯一id</param>
        public void Recycle(GameObject gameObject, int id)
        {
            IdCheck(id);
            if (_isTrace)
            {
                if (_active.TryGetValue(gameObject, out var saveId))
                {
                    if (saveId != id)
                    {
                        throw new ArgumentException($"输入的id:{id}与保存的id:{saveId}不一致");
                    }

                    _pool[id].Item2.Push(gameObject);
                    _active.Remove(gameObject);
                }
                else
                {
                    throw new ArgumentException($"GameObject:{gameObject.name} 不是从本池分配出去的");
                }
            }
            else
            {
                _pool[id].Item2.Push(gameObject);
            }
        }

        /// <summary>
        /// 清空对象池
        /// </summary>
        /// <param name="isForce">是否强制清空</param>
        /// <exception cref="InvalidOperationException">有未回收的对象</exception>
        public void Clear(bool isForce)
        {
            if (_isTrace)
            {
                if (isForce)
                {
                    Debug.LogWarning($"未回收全部对象就清空，你最好知道你在做什么");
                    foreach (var gameObject in _active.Keys)
                    {
                        Destroy(gameObject);
                    }
                }
                else
                {
                    if (_active.Count > 0)
                    {
                        throw new InvalidOperationException($"还有未回收的对象{_active.Count}个,不可以清空对象池");
                    }
                }

                _active.Clear();
            }

            foreach (var (_, stack) in _pool)
            {
                foreach (var gameObject in stack)
                {
                    Destroy(gameObject);
                }
            }

            _pool.Clear();
        }

        private GameObject Instantiate(GameObject prefab) { return UnityEngine.Object.Instantiate(prefab); }

        private void Destroy(GameObject gameObject) { UnityEngine.Object.Destroy(gameObject); }

        /// <summary>
        /// id检查
        /// </summary>
        private void IdCheck(int id)
        {
            if (id >= _pool.Count)
            {
                throw new ArgumentException($"不存在id:{id}");
            }
        }
    }
}