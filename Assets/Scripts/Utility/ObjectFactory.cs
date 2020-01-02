using System;

/// <summary>
/// 对象工厂
/// </summary>
public class ObjectFactory<T>
{
	private readonly Func<T, T> _factory;

	public T Template { get; }
	public int MakeCount { get; private set; }

	public event Action<T> OnDestruct;

	public ObjectFactory(T template, Func<T, T> factory)
	{
		Template = template;
		_factory = factory ?? throw new ArgumentNullException();
	}

	public ObjectFactory(T template, Func<T, T> factory, Action<T> destruct) : this(template, factory)
	{
		OnDestruct += destruct;
	}

	/// <summary>
	/// 构造对象
	/// </summary>
	public T Make()
	{
		var product = _factory(Template);
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
