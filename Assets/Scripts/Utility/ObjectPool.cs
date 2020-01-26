using System;
using System.Collections.Generic;

/// <summary>
/// 对象池
/// </summary>
/// <typeparam name="T">对象类型</typeparam>
/// <typeparam name="TK">分辨相同类型的不同对象</typeparam>
public class ObjectPool<T, TK>
{
    private readonly Stack<T> _pool = new Stack<T>();
    private readonly Dictionary<TK, T> _actives = new Dictionary<TK, T>();

    public ObjectFactory<T> Factory { get; }

    /// <summary>
    /// 池中剩余对象数量
    /// </summary>
    public int Count => _pool.Count;

    /// <summary>
    /// 分辨对象
    /// </summary>
    public event Func<T, TK> Distinguish;

    /// <summary>
    /// 预回收对象时触发
    /// </summary>
    public event Func<T, bool> OnPreRecycle;

    /// <summary>
    /// 回收对象时触发
    /// </summary>
    public event Action<T> OnRecycle;

    public ObjectPool(T template, Func<T, T> factory) { Factory = new ObjectFactory<T>(template, factory); }

    public ObjectPool(T template, Func<T, T> factory, int initCount) : this(template, factory)
    {
        for (var i = 0; i < initCount; i++)
        {
            _pool.Push(Factory.Make());
        }
    }

    /// <summary>
    /// 返回池中资源
    /// </summary>
    /// <returns>池中资源</returns>
    public T Get()
    {
        var result = _pool.Count > 0 ? _pool.Pop() : Factory.Make();
        var key = CallDistinguish(result);
        _actives.Add(key, result);
        return result;
    }

    /// <summary>
    /// 回收对象到池中
    /// </summary>
    /// <param name="obj">需要被回收的对象</param>
    /// <returns>是否回收成功</returns>
    public bool Recycle(T obj)
    {
        if (obj.Equals(default(T)))
        {
            return false;
        }

        var key = CallDistinguish(obj);
        if (!_actives.ContainsKey(key))
        {
            throw new ArgumentException($"{obj}不是从该池构造的");
        }

        if (OnPreRecycle != null)
        {
            if (!OnPreRecycle(obj))
            {
                return false;
            }
        }

        OnRecycle?.Invoke(obj);
        _pool.Push(obj);
        _actives.Remove(key);
        return true;
    }

    /// <summary>
    /// 释放池中资源
    /// </summary>
    /// <param name="remain">释放后剩余对象数量</param>
    public void TrimExcess(int remain)
    {
        if (remain < 0)
        {
            throw new ArgumentException("剩余数量不能小于0");
        }

        var count = _pool.Count;
        for (var i = 0; i < count - remain; i++)
        {
            Factory.Destruct(_pool.Peek());
            _pool.Pop();
        }

        _pool.TrimExcess();
    }

    private TK CallDistinguish(T t)
    {
        if (Distinguish != null)
        {
            return Distinguish(t);
        }

        throw new ArgumentNullException("OnGet事件不能为null");
    }
}