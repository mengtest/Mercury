using UnityEngine;

/// <summary>
/// 可挂载单例
/// </summary>
public abstract class MonoSingleton<T> : MonoBehaviour where T : Component
{
	protected static T _instance;

	/// <summary>
	/// 获取单例实例
	/// </summary>
	public static T Instance
	{
		get
		{
			if (_instance)
			{
				return _instance;
			}

			_instance = FindObjectOfType<T>();
			if (_instance)
			{
				return _instance;
			}

			var obj = new GameObject { name = typeof(T).Name };
			_instance = obj.AddComponent<T>();
			return _instance;
		}
	}

	protected virtual void Awake()
	{
		if (!_instance)
		{
			_instance = this as T;
			DontDestroyOnLoad(gameObject);
		}
	}

	protected virtual void Update()
	{

	}
}
