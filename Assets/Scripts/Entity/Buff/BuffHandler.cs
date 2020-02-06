using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Buff处理者
/// </summary>
public class BuffHandler
{
    private readonly Dictionary<Type, BuffFlyweightDot> _dots; //TODO:哈希堆？
    private readonly Dictionary<Type, BuffFlyweightState> _states;
    private readonly IBuffable _holder;

    private readonly List<BuffFlyweightDot> _modifyDots;
    private readonly List<BuffFlyweightState> _modifyStates;

    public BuffHandler(IBuffable holder, int capacity = 2)
    {
        _holder = holder;
        _dots = new Dictionary<Type, BuffFlyweightDot>(capacity);
        _modifyDots = new List<BuffFlyweightDot>(capacity);
        _states = new Dictionary<Type, BuffFlyweightState>(capacity);
        _modifyStates = new List<BuffFlyweightState>(capacity);
    }

    public void OnUpdate()
    {
        var nowTime = Time.time;
        foreach (var dot in _dots.Values)
        {
            if (!(nowTime >= dot.nextTime)) //没到下次触发时间
            {
                continue;
            }

            dot.prototype.OnTrigger(_holder, dot);
            if (dot.TriggerCount - 1 >= 0) //小于0是无限
            {
                _modifyDots.Add(dot);
            }
        }

        foreach (var modify in _modifyDots)
        {
            var temp = modify.AfterTrigger();
            if (temp.TriggerCount == 0) //触发次数归0直接删除
            {
                _dots.Remove(modify.prototype.GetType());
                continue;
            }

            _dots[modify.prototype.GetType()] = temp;
        }

        _modifyDots.Clear();

        foreach (var state in _states.Values)
        {
            if (state.expireTime <= nowTime) //到期时间小于现在，过期了
            {
                _modifyStates.Add(state);
            }
        }

        foreach (var state in _modifyStates)
        {
            _states.Remove(state.prototype.GetType());
        }

        _modifyStates.Clear();
    }

    public void Add(BuffFlyweightDot dot) { Add(dot, _dots); }

    public void Add(BuffFlyweightState state) { Add(state, _states); }

    private void Add<T>(T buff, IDictionary<Type, T> dict) where T : struct, IBuffFlyweight<T>
    {
        if (dict.TryGetValue(buff.Prototype.GetType(), out var exist))
        {
            var tmp = buff.Prototype.Merge(ref buff, ref exist);
            dict[buff.Prototype.GetType()] = tmp;
            buff.Prototype.OnRepeatAdd(_holder, in tmp);
        }
        else
        {
            buff.Prototype.OnFirstAdd(_holder, in buff);
            dict.Add(buff.Prototype.GetType(), buff);
        }
    }

    public bool RemoveDot<T>() where T : DotBuff
    {
        if (!_dots.TryGetValue(typeof(T), out var dot))
        {
            return false;
        }

        return dot.prototype.OnRemove(_holder, in dot) && _dots.Remove(typeof(T));
    }

    public bool RemoveState<T>() where T : StateBuff
    {
        if (!_states.TryGetValue(typeof(T), out var dot))
        {
            return false;
        }

        return dot.prototype.OnRemove(_holder, in dot) && _states.Remove(typeof(T));
    }

    public bool ContainsDot<T>() where T : DotBuff { return _dots.ContainsKey(typeof(T)); }

    public bool ContainsState<T>() where T : StateBuff { return _states.ContainsKey(typeof(T)); }

    public bool TryGetDot<T>(out BuffFlyweightDot dot) { return _dots.TryGetValue(typeof(T), out dot); }

    public bool TryGetState<T>(out BuffFlyweightState state) { return _states.TryGetValue(typeof(T), out state); }
}