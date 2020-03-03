using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace Tests
{
    public class EffectPoolTest
    {
        [Test]
        public void EffectPoolTestSimplePasses()
        {
            var prefab = new GameObject("test"); //预制体

            var pool = new EffectController(); //池

            var ins0 = pool.CreateEffectAt(new Vector2(), prefab); //获取一个复制体
            Assert.True(pool._effectList.Count == 1); //池是不是1
            Assert.True(ins0.activeSelf); //复制体是不是活动中

            pool.AddEffectToQueue(ins0); //放回池中
            Assert.True(pool._effectList.Count == 1);
            Assert.True(pool._effectList[0]._queue.Count == 1);
        }

        public class EffectController
        {
            public struct EffectAggregate
            {
                public GameObject _effectName;
                public Queue<GameObject> _queue;
            }

            public List<EffectAggregate> _effectList;

            public EffectController() { _effectList = new List<EffectAggregate>(); }

            /// <summary>
            /// 创建一个特效在指定位置，注意这里特效创建结束之后不会被删除，会在当前地图中被保存，直到进入下一张地图。
            /// </summary>
            /// <param name="vector">位置</param>
            /// <param name="obj">预制体特效</param>
            public GameObject CreateEffectAt(Vector2 vector, GameObject obj)
            {
                int index = FindElement(obj);
                GameObject go;
                if (index == -1)
                {
                    go = UnityEngine.GameObject.Instantiate(obj); //创建特效
                    go.transform.position = vector; //将特效移动至某个地方

                    EffectAggregate ef;
                    ef._queue = new Queue<GameObject>(); //在List里面新建一个。
                    ef._effectName = obj;
                    _effectList.Add(ef);
                }
                else
                {
                    if (_effectList[index]._queue.Count == 0)
                    {
                        go = UnityEngine.GameObject.Instantiate(obj); //创建特效
                        go.transform.position = vector; //将特效移动至某个地方
                    }
                    else
                    {
                        go = _effectList[index]._queue.Dequeue();
                        go.transform.position = vector;
                        go.SetActive(true);
                    }
                }

                return go;
            }

            /// <summary>
            /// 查找obj在_effectList集合里面的位置，如果不存在则返回-1
            /// </summary>
            /// <param name="obj">要查找元素的GameObject</param>
            /// <returns></returns>
            private int FindElement(GameObject obj)
            {
                for (int i = 0; i < _effectList.Count; i++)
                {
                    var effName = _effectList[i]._effectName;
                    var result = effName == obj;
                    Debug.Log($"{obj.name}和{effName.name}比较结果:{result}");
                    if (result)
                    {
                        return i;
                    }
                }

                return -1;
            }

            /// <summary>
            /// 使用完的特效使用该函数可以存到缓存内，等下次使用的时候再次拿出来。
            /// </summary>
            /// <param name="obj">要储存的特效的GameObject</param>
            public void AddEffectToQueue(GameObject obj) { _effectList[FindElement(obj)]._queue.Enqueue(obj); }
        }
    }
}