using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectController
{
    private struct EffectAggregate
    {
        public GameObject _effectName;
        public Queue<GameObject> _queue;
    }

    private List<EffectAggregate> _effectList;

    /// <summary>
    /// 创建一个特效在指定位置，注意这里特效创建结束之后不会被删除，会在当前地图中被保存，直到进入下一张地图。
    /// </summary>
    /// <param name="vector">位置</param>
    /// <param name="obj">预制体特效</param>
    [System.Obsolete]
    public void CreateEffectAt(Vector2 vector,GameObject obj)
    {
        int index = FindElemnet(obj);
        if (index == -1)
        {
            GameObject go = UnityEngine.GameObject.Instantiate(obj);//创建特效
            go.transform.position = vector;//将特效移动至某个地方

            EffectAggregate ef;
            ef._queue = new Queue<GameObject>();//在List里面新建一个。
            ef._effectName = obj;
            _effectList.Add(ef);

        }
        else
        {
            if (_effectList[index]._queue.Count == 0)
            {
                GameObject go = UnityEngine.GameObject.Instantiate(obj);//创建特效
                go.transform.position = vector;//将特效移动至某个地方
            }
            else
            {
                GameObject use = _effectList[index]._queue.Dequeue();
                use.transform.position = vector;
            }
        }
    }
     /// <summary>
     /// 查找obj在_effectList集合里面的位置，如果不存在则返回-1
     /// </summary>
     /// <param name="obj">要查找元素的GameObject</param>
     /// <returns></returns>
    private int FindElemnet(GameObject obj)
    {
        for (int i = 0; i < _effectList.Count; i ++)
        {
            if (_effectList[i]._effectName == obj)
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
    public void AddEffectToQueue(GameObject obj)
    {
        _effectList[FindElemnet(obj)]._queue.Enqueue(obj);
    }
}
