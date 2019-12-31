using System;

/// <summary>
/// 对象工厂
/// </summary>
public class ObjectFactory<T>
{
	private readonly Func<T> _factory;

	public int MakeCount { get; private set; }

	public event Action<T> OnDestruct;

	public ObjectFactory(Func<T> factory)
	{
		_factory = factory ?? throw new ArgumentNullException();
	}

	public ObjectFactory(Func<T> factory, Action<T> destruct) : this(factory)
	{
		OnDestruct += destruct;
	}

	/// <summary>
	/// 构造对象
	/// </summary>
	public T Make()
	{
		var product = _factory();
		MakeCount += 1;
		return product;
	}

	/// <summary>
	/// 析构对象
	/// </summary>
	/// <param name="obj"></param>
	public void Destruct(T obj)
	{
		OnDestruct?.Invoke(obj);
		MakeCount -= 1;
	}
}
