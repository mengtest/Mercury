using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace Tests
{
    public class ObjectPoolTest
    {
        private ObjectPool _pool = new ObjectPool(false);

        [Test]
        public void Simple()
        {
            var p1 = new GameObject("114");
            var p2 = new GameObject("514");
            var key1 = _pool.Allocate(p1, 5);
            var key2 = _pool.Allocate(p2, 10);
            Assert.True(_pool._pool.Count == 2); //是否分配成功
            Assert.True(_pool._pool[0].Item2.Count == 5); //数量对不对
            Assert.True(_pool._pool[1].Item2.Count == 10);
            var t11 = _pool.Get(key1);
            var t21 = _pool.Get(key2);
            Assert.True(t11.name == "114(Clone)"); //名字对不对
            Assert.True(t21.name == "514(Clone)");
            Assert.True(_pool._pool[0].Item2.Count == 4); //是否成功获取
            Assert.True(_pool._pool[1].Item2.Count == 9);
            _pool.Recycle(t11, key1);
            _pool.Recycle(t21, key2);
            t11 = null;
            t21 = null;
            Assert.True(_pool._pool[0].Item2.Count == 5); //是否成功回收
            Assert.True(_pool._pool[1].Item2.Count == 10);
            _pool.Clear(true); //清空
            Assert.True(_pool._pool.Count == 0);
        }

        public class ObjectPool
        {
            public readonly List<(GameObject, Stack<GameObject>)> _pool;
            public readonly bool _isTrace;
            public readonly Dictionary<GameObject, int> _active;

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

            private void Destroy(GameObject gameObject) { }

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
}