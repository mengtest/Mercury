using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Buff处理者
/// </summary>
public class BuffHandler
{
    private readonly Dictionary<string, BuffStack> _activeBuffs;
    private readonly List<BuffStack> _modify;
    private readonly IBuffable _holder;

    public BuffStack this[string name] => _activeBuffs[name];

    public BuffHandler(IBuffable holder, int capacity = 2)
    {
        _holder = holder;
        _activeBuffs = new Dictionary<string, BuffStack>(capacity);
        _modify = new List<BuffStack>(capacity);
    }

    public void OnUpdate()
    {
        //Debug.Log($"Buff数量:{_activeBuffs.Count}");
        var now = Time.time;
        foreach (var buff in _activeBuffs.Values)
        {
            //Debug.Log($"Buff:{buff}");
            if (now < buff.nextTime)
            {
                continue;
            }

            buff.prototype.OnTrigger(_holder, buff);
            _modify.Add(buff);
        }

        foreach (var buff in _modify)
        {
            if (buff.triggerCount - 1 == 0)
            {
                var name = buff.prototype.RegisterName.ToString();
                _activeBuffs.Remove(name);
            }
            else
            {
                var temp = buff.AfterTrigger();
                var name = temp.prototype.RegisterName.ToString();
                _activeBuffs[name] = temp;
            }
        }

        _modify.Clear();
    }

    public void Add(BuffStack buff)
    {
        var prototype = buff.prototype;
        var name = prototype.RegisterName.ToString();
        if (_activeBuffs.TryGetValue(name, out var exist))
        {
            var temp = prototype.Merge(buff, exist);
            _activeBuffs[name] = temp;
            prototype.OnRepeatAdd(_holder, temp);
        }
        else
        {
            prototype.OnFirstAdd(_holder, buff);
            _activeBuffs.Add(name, buff);
        }
    }

    public bool Remove(string name)
    {
        if (!_activeBuffs.TryGetValue(name, out var buff))
        {
            return false;
        }

        if (!buff.prototype.OnRemove(_holder, buff))
        {
            return false;
        }

        if (!_activeBuffs.Remove(name))
        {
            throw new InvalidOperationException();
        }

        return true;
    }

    public bool Contains(string name) { return _activeBuffs.ContainsKey(name); }

    public bool TryGet(string name, out BuffStack buff) { return _activeBuffs.TryGetValue(name, out buff); }
}