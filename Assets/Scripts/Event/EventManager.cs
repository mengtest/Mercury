using System;
using System.Collections.Generic;

/// <summary>
/// 极其简单的事件系统
/// </summary>
public class EventManager : Singleton<EventManager>
{
    /// <summary>
    /// key:事件类型
    /// value:订阅者
    /// </summary>
    private readonly Dictionary<Type, Delegate> _events;

    private EventManager() { _events = new Dictionary<Type, Delegate>(); }

    /// <summary>
    /// 订阅事件
    /// </summary>
    /// <param name="handler">委托，如果是null的话，你最好知道你在干什么</param>
    /// <typeparam name="T">事件类型</typeparam>
    public void AddListener<T>(EventHandler<T> handler) where T : EventArgs
    {
        if (_events.TryGetValue(typeof(T), out var dlgt))
        {
            _events[typeof(T)] = Delegate.Combine(dlgt, handler);
        }
        else
        {
            _events.Add(typeof(T), handler);
        }
    }

    /// <summary>
    /// 发布事件
    /// </summary>
    /// <param name="sender">发布者</param>
    /// <param name="args">事件参数</param>
    /// <typeparam name="T">事件类型</typeparam>
    public void Publish<T>(object sender, T args) where T : EventArgs
    {
        if (_events.TryGetValue(typeof(T), out var dlgt))
        {
            dlgt.DynamicInvoke(sender, args);
        }
    }
}