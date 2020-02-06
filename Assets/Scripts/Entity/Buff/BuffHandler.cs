using System.Collections.Generic;
using UnityEngine;

public class BuffHandler
{
    private readonly Dictionary<DotBuff, BuffFlyweightDot> _dots;
    private readonly IBuffable _holder;

    private List<BuffFlyweightDot> _modifyDots;

    public BuffHandler(IBuffable holder, int capacity = 2)
    {
        _holder = holder;
        _dots = new Dictionary<DotBuff, BuffFlyweightDot>(capacity);
        _modifyDots = new List<BuffFlyweightDot>(capacity);
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
                _dots.Remove(modify.prototype);
                continue;
            }

            _dots[modify.prototype] = temp;
        }

        _modifyDots.Clear();
    }

    public void Add(BuffFlyweightDot dot)
    {
        if (_dots.TryGetValue(dot.prototype, out var exist))
        {
            _dots[dot.prototype] = dot.prototype.Merge(ref dot, ref exist);
            dot.prototype.OnRepeatAdd(_holder, in dot);
        }
        else
        {
            dot.prototype.OnFirstAdd(_holder, in dot);
            _dots.Add(dot.prototype, dot);
        }
    }

    public bool RemoveDot<T>() where T : DotBuff
    {
        var ins = BuffFactory.Instance.Dots[typeof(T)];
        if (!_dots.TryGetValue(ins, out var dot))
        {
            return false;
        }

        return dot.prototype.OnRemove(_holder, in dot) && _dots.Remove(ins);
    }

    public bool ContainsDot<T>() where T : DotBuff { return _dots.ContainsKey(BuffFactory.Instance.Dots[typeof(T)]); }

    public bool TryGetDot<T>(out BuffFlyweightDot dot)
    {
        return _dots.TryGetValue(BuffFactory.Instance.Dots[typeof(T)], out dot);
    }
}