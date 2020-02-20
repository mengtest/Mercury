using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Buff处理者，处理Buff的增删改查
/// </summary>
public class BuffHandler
{
    /// <summary>
    /// 活动中的buff的容器
    /// </summary>
    private readonly Dictionary<string, BuffStack> _activeBuffs;

    /// <summary>
    /// 每次遍历活动中buff容器时，需要修改的buff的容器
    /// </summary>
    private readonly List<BuffStack> _modify;

    /// <summary>
    /// 持有本buff处理者的实体
    /// </summary>
    private readonly IBuffable _holder;

    public BuffStack this[string name] => _activeBuffs[name];

    public BuffHandler(IBuffable holder, int capacity = 2)
    {
        _holder = holder;
        _activeBuffs = new Dictionary<string, BuffStack>(capacity);
        _modify = new List<BuffStack>(capacity);
    }

    /// <summary>
    /// 每帧调用
    /// </summary>
    public void OnUpdate()
    {
        //Debug.Log($"Buff数量:{_activeBuffs.Count}");
        var now = Time.time;
        foreach (var buff in _activeBuffs.Values) //遍历活动中buff容器
        {
            //Debug.Log($"Buff:{buff}");
            if (now < buff.nextTime) // 比较当前时间和buff下一次触发时间
            {
                continue; //未到触发时间，跳过
            }

            buff.prototype.OnTrigger(_holder, buff); //触发buff效果
            _modify.Add(buff); //buff需要修改，加入修改容器
        }

        foreach (var buff in _modify) //遍历修改容器
        {
            if (buff.triggerCount - 1 == 0) //如果该buff的触发次数归零了
            {
                var name = buff.prototype.RegisterName.ToString(); //获取buff id
                _activeBuffs.Remove(name); //移除该活动中buff
            }
            else
            {
                var temp = buff.AfterTrigger(); //获取触发后的buff
                var name = temp.prototype.RegisterName.ToString();
                _activeBuffs[name] = temp; //修改活动中buff容器
            }
        }

        _modify.Clear(); //清空修改容器
    }

    /// <summary>
    /// 添加一个Buff
    /// </summary>
    /// <param name="buff">buff实例</param>
    public void Add(BuffStack buff)
    {
        var prototype = buff.prototype;
        var name = prototype.RegisterName.ToString();
        if (_activeBuffs.TryGetValue(name, out var exist)) //如果活动中buff容器内已经有这个buff了
        {
            var temp = prototype.Merge(buff, exist); //合并
            _activeBuffs[name] = temp; //将合并后buff放入容器
            prototype.OnRepeatAdd(_holder, temp); //调用再次添加
        }
        else
        {
            prototype.OnFirstAdd(_holder, buff); //调用初次添加
            _activeBuffs.Add(name, buff); //加入活动中buff容器
        }
    }

    /// <summary>
    /// 移除一个活动中buff
    /// </summary>
    /// <param name="name">buff id的string形式</param>
    /// <returns>是否删除成功</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public bool Remove(string name)
    {
        if (!_activeBuffs.TryGetValue(name, out var buff)) //尝试从容器获取buff
        {
            return false; //未获取到
        }

        if (!buff.prototype.OnRemove(_holder, buff)) //调用删除时触发
        {
            return false; //不能删buff
        }

        if (!_activeBuffs.Remove(name)) //理论上不可能删除失败
        {
            throw new InvalidOperationException();
        }

        return true;
    }

    /// <summary>
    /// 查询是否拥有buff
    /// </summary>
    /// <param name="name">buff id</param>
    /// <returns>是否拥有buff</returns>
    public bool Contains(string name) { return _activeBuffs.ContainsKey(name); }

    /// <summary>
    /// 尝试获取buff
    /// </summary>
    /// <param name="name">buff id</param>
    /// <param name="buff">如果获取成功则返回buff实例，否则返回null</param>
    /// <returns>是否获取到buff</returns>
    public bool TryGet(string name, out BuffStack buff) { return _activeBuffs.TryGetValue(name, out buff); }
}