using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

/// <summary>
/// 极其简单的事件系统
/// 注册的事件类型必须继承自EventArgs
/// </summary>
public class EventManager : Singleton<EventManager>
{
    /// <summary>
    /// key:事件类型
    /// value:订阅者
    /// </summary>
    private readonly Dictionary<Type, Delegate> _events;

    private EventManager() { _events = new Dictionary<Type, Delegate>(); }

    public void Init()
    {
        foreach (var type in typeof(EventManager).Assembly.GetTypes())
        {
            var attr = type.GetCustomAttribute(typeof(EventSubscriberAttribute));
            if (attr == null)
            {
                continue;
            }

            var methods = type
                .GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(m =>
                {
                    if (m.GetCustomAttribute(typeof(SubscribeAttribute)) == null)
                    {
                        return false;
                    }

                    if (m.ReturnType != typeof(void))
                    {
                        Debug.LogError($"方法{type.FullName}:{m.Name}形返回值不是void");
                        return false;
                    }

                    var param = m.GetParameters();
                    if (param.Length != 2)
                    {
                        Debug.LogError($"方法{type.FullName}:{m.Name}形参数量不是2");
                        return false;
                    }

                    if (param[0].ParameterType != typeof(object))
                    {
                        Debug.LogError($"方法{type.FullName}:{m.Name}第一个形参类型不是object");
                        return false;
                    }

                    if (param[1].ParameterType.IsSubclassOf(typeof(EventArgs)))
                    {
                        return true;
                    }

                    Debug.LogError($"方法{type.FullName}:{m.Name}第一个形参类型的不是{typeof(EventArgs).FullName}的子类");
                    return false;
                });
            foreach (var m in methods)
            {
                var typeName = m.GetParameters()[1].ParameterType;
                var typeFullName = $"System.EventHandler`1[{typeName.FullName}]";
                var t = Type.GetType(typeFullName);
                if (t == null)
                {
                    Debug.LogWarning($"无法获取类型{typeFullName}");
                    continue;
                }

                var d = Delegate.CreateDelegate(t, m);
                if (_events.TryGetValue(typeName, out var dlgt))
                {
                    _events[typeName] = Delegate.Combine(dlgt, d);
                }
                else
                {
                    _events.Add(typeName, d);
                }
            }
        }
    }

    /// <summary>
    /// 订阅事件
    /// </summary>
    /// <param name="handler">委托，如果是null的话，你最好知道你在干什么</param>
    /// <typeparam name="T">事件类型</typeparam>
    public void Subscribe<T>(EventHandler<T> handler) where T : EventArgs
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

    /// <summary>
    /// 取消订阅
    /// </summary>
    /// <param name="handler">委托</param>
    /// <typeparam name="T"></typeparam>
    public void Unsubscribe<T>(EventHandler<T> handler) where T : EventArgs
    {
        if (_events.TryGetValue(typeof(T), out var events))
        {
            _events[typeof(T)] = Delegate.Remove(events, handler);
        }
        else
        {
            throw new ArgumentException($"未订阅{typeof(T).FullName}类型的事件");
        }
    }

    /// <summary>
    /// 取消订阅所有T类型的事件
    /// </summary>
    /// <exception cref="ArgumentException">未订阅事件</exception>
    public void Unsubscribe<T>() where T : EventArgs
    {
        if (_events.TryGetValue(typeof(T), out var events))
        {
            _events.Remove(typeof(T));
        }
        else
        {
            throw new ArgumentException($"未订阅{typeof(T).FullName}类型的事件");
        }
    }
}