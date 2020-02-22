using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

/// <summary>
/// 基于事件的依赖注入
/// </summary>
public class IoCContainer : Singleton<IoCContainer>
{
    private readonly Dictionary<Type, Dictionary<Type, Action<object>>> _container;

    private IoCContainer() { _container = new Dictionary<Type, Dictionary<Type, Action<object>>>(); }

    public void Init()
    {
        EventManager.Instance.Publish(this, new InjectEvent.InitContainer(this));
        EventManager.Instance.Unsubscribe<InjectEvent.InitContainer>();
    }

    public void Register<TClass, TField>(Action<object> inject)
    {
        var clz = typeof(TClass);
        var fie = typeof(TField);
        if (_container.TryGetValue(clz, out var fieldDict))
        {
            if (fieldDict.ContainsKey(fie))
            {
                throw new InvalidOperationException();
            }

            fieldDict.Add(fie, inject);
        }
        else
        {
            _container.Add(clz, new Dictionary<Type, Action<object>> {{fie, inject}});
        }
    }

    public void Inject<TClass>(Type fieldType, object param) { _container[typeof(TClass)][fieldType].Invoke(param); }

    public void InjectFields<TClass, TField>(object ins, params object[] fieldValues)
    {
        var clz = typeof(TClass);
        if (!_container.TryGetValue(clz, out var dict))
        {
            return;
        }

        var fie = typeof(TField);
        if (!dict.ContainsKey(fie))
        {
            return;
        }

        var fields = ins.GetType()
            .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(f => f.FieldType == fie && f.GetCustomAttribute<InjectAttribute>() != null)
            .ToArray();
        if (fieldValues.Length != fields.Length)
        {
            Debug.LogWarning($"需要注入的数量{fields.Length}与参数数量{fieldValues.Length}不一致");
        }

        for (var i = 0; i < fields.Length; i++)
        {
            fields[i].SetValue(ins, fieldValues[i]);
        }
    }
}