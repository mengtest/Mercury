using System;
using System.Collections.Generic;

/// <summary>
/// 对象池|
/// 析构对象请将对象返回入池中统一析构，否则计数会出问题
/// </summary>
public class ObjectPool<T>
{
	private readonly Stack<T> _pool = new Stack<T>();

	public ObjectFactory<T> Factory { get; }
	/// <summary>
	/// 池中剩余对象数量
	/// </summary>
	public int Count => _pool.Count;

	/// <summary>
	/// 从池中获取对象时触发
	/// </summary>
	public event Action<T> OnGet;
	/// <summary>
	/// 预回收对象时触发，可用于判断该对象是否可被池回收
	/// </summary>
	public event Func<T, bool> OnPreRecycle;
	/// <summary>
	/// 回收对象时触发
	/// </summary>
	public event Action<T> OnRecycle;

	public ObjectPool(Func<T> factory)
	{
		Factory = new ObjectFactory<T>(factory ?? throw new ArgumentNullException());
	}

	public ObjectPool(Func<T> factory, int initCount) : this(factory)
	{
		for (int i = 0; i < initCount; i++)
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
		if (obj == null)
		{
			return false;
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
