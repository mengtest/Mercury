using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Buff处理者
/// </summary>
public class BuffHandler //TODO：将状态和Dot合并
{
    private readonly Dictionary<string, BuffFlyweightDot> _dots;
    private readonly Dictionary<string, BuffFlyweightState> _states;
    private readonly IBuffable _holder;

    private readonly List<BuffFlyweightDot> _modifyDots;
    private readonly List<BuffFlyweightState> _modifyStates;

    public BuffHandler(IBuffable holder, int capacity = 2)
    {
        _holder = holder;
        _dots = new Dictionary<string, BuffFlyweightDot>(capacity);
        _modifyDots = new List<BuffFlyweightDot>(capacity);
        _states = new Dictionary<string, BuffFlyweightState>(capacity);
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
            var name = modify.GetPrototype<BuffFlyweightDot>().Name;
            if (temp.TriggerCount == 0) //触发次数归0直接删除
            {
                _dots.Remove(name);
                continue;
            }

            _dots[name] = temp;
        }

        _modifyDots.Clear();

        foreach (var state in _states.Values)
        {
            if (state.ExpireTime <= nowTime) //到期时间小于现在，过期了
            {
                _modifyStates.Add(state);
            }
        }

        foreach (var state in _modifyStates)
        {
            _states.Remove(state.GetPrototype<BuffFlyweightState>().Name);
        }

        _modifyStates.Clear();
    }

    public void Add(BuffFlyweightDot dot) { Add(dot, _dots); }

    public void Add(BuffFlyweightState state) { Add(state, _states); }

    private void Add<T>(T buff, IDictionary<string, T> dict) where T : struct, IBuffFlyweight
    {
        var prototype = buff.GetPrototype<T>();
        var type = prototype.Name;
        if (dict.TryGetValue(type, out var exist))
        {
            var tmp = prototype.Merge(ref buff, ref exist);
            dict[type] = tmp;
            prototype.OnRepeatAdd(_holder, in tmp);
        }
        else
        {
            prototype.OnFirstAdd(_holder, in buff);
            dict.Add(type, buff);
        }
    }

    public bool RemoveDot(string name)
    {
        if (!_dots.TryGetValue(name, out var dot))
        {
            return false;
        }

        return dot.prototype.OnRemove(_holder, in dot) && _dots.Remove(name);
    }

    public bool RemoveState(string name)
    {
        if (!_states.TryGetValue(name, out var dot))
        {
            return false;
        }

        return dot.prototype.OnRemove(_holder, in dot) && _states.Remove(name);
    }

    public bool ContainsDot(string name) { return _dots.ContainsKey(name); }

    public bool ContainsState(string name) { return _states.ContainsKey(name); }

    public bool TryGetDot(string name, out BuffFlyweightDot dot) { return _dots.TryGetValue(name, out dot); }

    public bool TryGetState(string name, out BuffFlyweightState state) { return _states.TryGetValue(name, out state); }

    public BuffFlyweightDot GetDot(string name) { return _dots[name]; }

    public BuffFlyweightState GetState(string name) { return _states[name]; }
}