using System;
using System.Collections.Generic;

/// <summary>
/// 对象池
/// </summary>
/// <typeparam name="T">对象类型</typeparam>
/// <typeparam name="TDistinguishKey">分辨相同类型的不同对象</typeparam>
public class ObjectPool<T, TDistinguishKey>
{
    private readonly Stack<T> _pool = new Stack<T>();
    private readonly ICollection<TDistinguishKey> _activeObjs;

    public ObjectFactory<T> Factory { get; }

    /// <summary>
    /// 池中剩余对象数量
    /// </summary>
    public int Count => _pool.Count;

    /// <summary>
    /// 分辨对象
    /// </summary>
    private readonly Func<T, TDistinguishKey> _distinguish;

    /// <summary>
    /// 获取对象时触发
    /// </summary>
    public event Action<T> OnGet;

    /// <summary>
    /// 回收对象时触发
    /// </summary>
    public event Action<T> OnRecycle;

    public ObjectPool(
        ICollection<TDistinguishKey> activeObjCollection,
        Func<T> factory,
        Func<T, TDistinguishKey> distinguish)
    {
        Factory = new ObjectFactory<T>(factory);
        _activeObjs = activeObjCollection;
        _distinguish = distinguish ?? throw new ArgumentNullException();
    }

    public ObjectPool(Func<T> factory, Func<T, TDistinguishKey> distinguish) : this(
        new HashSet<TDistinguishKey>(),
        factory,
        distinguish)
    {
    }

    public ObjectPool(int initCount, Func<T> factory, Func<T, TDistinguishKey> distinguish) :
        this(factory, distinguish)
    {
        InitPool(initCount);
    }

    public ObjectPool(
        ICollection<TDistinguishKey> activeObjCollection,
        int initCount,
        Func<T> factory,
        Func<T, TDistinguishKey> distinguish) : this(
        activeObjCollection,
        factory,
        distinguish)
    {
        InitPool(initCount);
    }

    private void InitPool(int initCount)
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
        var key = _distinguish(result);
        _activeObjs.Add(key);
        OnGet?.Invoke(result);
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

        var key = _distinguish(obj);
        if (!_activeObjs.Contains(key))
        {
            throw new ArgumentException($"{obj}不是从该池构造的");
        }

        OnRecycle?.Invoke(obj);
        _pool.Push(obj);
        _activeObjs.Remove(key);
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
}