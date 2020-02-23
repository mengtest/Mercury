using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Mercury
{
    /// <summary>
    /// 简单事件系统
    /// 注册的事件类型必须继承自EventArgs
    /// </summary>
    public class EventSystem
    {
        /// <summary>
        /// key:事件类型
        /// value:订阅者
        /// </summary>
        private readonly Dictionary<Type, Delegate> _events;

        private readonly HashSet<object> _subscribedObject;
        private readonly Dictionary<Type, (Type, MethodInfo)[]> _canSubscribeMethodList;

        public EventSystem()
        {
            _events = new Dictionary<Type, Delegate>();
            _subscribedObject = new HashSet<object>();
            _canSubscribeMethodList = new Dictionary<Type, (Type, MethodInfo)[]>();
        }

        public void Init() { }

        /// <summary>
        /// 订阅事件
        /// </summary>
        /// <param name="handler">委托，如果是null的话，你最好知道你在干什么</param>
        /// <typeparam name="T">事件类型</typeparam>
        public void Subscribe<T>(EventHandler<T> handler) where T : EventArgs { AddDelegate(typeof(T), handler); }

        private static Delegate CreateDelegate(object instance, Type delegateType, MethodInfo method)
        {
            return method.IsStatic
                ? Delegate.CreateDelegate(delegateType, method)
                : Delegate.CreateDelegate(delegateType, instance, method);
        }

        /// <summary>
        /// 订阅事件，订阅事件的方法需要特性<see cref="SubscribeAttribute"/>
        /// </summary>
        /// <param name="instance">需要订阅事件的实例</param>
        public void Subscribe(object instance)
        {
            if (_subscribedObject.Contains(instance))
            {
                Debug.LogWarning($"已经订阅过的实例{instance}");
                return;
            }

            var type = instance.GetType();
            if (_canSubscribeMethodList.TryGetValue(type, out var methodList))
            {
                foreach (var (delegateType, method) in methodList)
                {
                    var created = CreateDelegate(instance, delegateType, method);
                    AddDelegate(delegateType, created);
                }

                return;
            }

            var selected = type
                .GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(method => method.GetCustomAttribute<SubscribeAttribute>() != null);
            var canSubscribeMethodList = new List<(Type, MethodInfo)>();
            foreach (var method in selected)
            {
                var param = method.GetParameters();
                if (param.Length != 2)
                {
                    throw new ArgumentException($"事件方法{type.FullName}::{method.Name}的参数个数必须为2");
                }

                if (param[0].ParameterType != typeof(object))
                {
                    throw new ArgumentException($"事件方法{type.FullName}:{method.Name}第一个形参类型不是{typeof(object).FullName}");
                }

                if (!param[1].ParameterType.IsSubclassOf(typeof(EventArgs)))
                {
                    throw new ArgumentException($"事件方法{type.FullName}:{method.Name}第二个形参类型不是{typeof(EventArgs).FullName}的子类");
                }

                var paramType = param[1].ParameterType;
                var delegateFullName = $"System.EventHandler`1[{paramType.FullName}]";
                var delegateType = Type.GetType(delegateFullName);
                if (delegateType == null)
                {
                    throw new ArgumentException($"无法获取委托类型{delegateFullName}");
                }

                var created = CreateDelegate(instance, delegateType, method);
                AddDelegate(delegateType, created);
                canSubscribeMethodList.Add((delegateType, method));
            }

            _subscribedObject.Add(instance);
            _canSubscribeMethodList.Add(type, canSubscribeMethodList.ToArray());
        }

        private void AddDelegate(Type delegateType, Delegate added)
        {
            if (_events.TryGetValue(delegateType, out var dlgt))
            {
                _events[delegateType] = Delegate.Combine(dlgt, added);
            }
            else
            {
                _events.Add(delegateType, added);
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
            if (!_events.TryGetValue(typeof(T), out var dlgt))
            {
                return;
            }

            try
            {
                dlgt.DynamicInvoke(sender, args);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private static bool Unsubscribe<T>(IDictionary<Type, Delegate> delegateDict, EventHandler<T> handler) where T : EventArgs
        {
            var delegateType = typeof(T);
            if (!delegateDict.TryGetValue(delegateType, out var events))
            {
                return false;
            }

            delegateDict[delegateType] = Delegate.Remove(events, handler);
            return true;
        }

        /// <summary>
        /// 取消订阅
        /// </summary>
        /// <param name="handler">委托</param>
        /// <typeparam name="T"></typeparam>
        public bool Unsubscribe<T>(EventHandler<T> handler) where T : EventArgs { return Unsubscribe(_events, handler); }

        /// <summary>
        /// 取消订阅所有T类型的事件
        /// </summary>
        /// <exception cref="ArgumentException">未订阅事件</exception>
        public bool Unsubscribe<T>() where T : EventArgs { return _events.Remove(typeof(T)); }
    }
}