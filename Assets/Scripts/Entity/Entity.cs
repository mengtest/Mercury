using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 实体基类
/// </summary>
public abstract class Entity : MonoBehaviour
{
	/// <summary>
	/// 血量
	/// </summary>
	[SerializeField]
	protected float _healthPoint;
	/// <summary>
	/// 最大血量
	/// </summary>
	[SerializeField]
	protected float _maxHealthPoint;
	/// <summary>
	/// 每秒血量变化
	/// </summary>
	protected float _hpRecoverPerSec;
	/// <summary>
	/// 死亡后实体消失时间
	/// </summary>
	protected float _deadBodySurviveTime;
	/// <summary>
	/// 属性容器
	/// </summary>
	protected readonly Dictionary<Type, IEntityProperty> _properties = new Dictionary<Type, IEntityProperty>();
	/// <summary>
	/// 物理系统
	/// </summary>
	protected readonly List<IEntitySystem> _physicalSystems = new List<IEntitySystem>();
	/// <summary>
	/// 普通系统
	/// </summary>
	protected readonly List<IEntitySystem> _normalSystems = new List<IEntitySystem>();

	public float HealthPoint { get => _healthPoint; }
	public float MaxHealthPoint { get => _maxHealthPoint; }
	public float HpRecoverPerSec { get => _hpRecoverPerSec; }
	public float DeadBodySurviveTime { get => _deadBodySurviveTime; }

	protected virtual void Awake()
	{
	}

	protected virtual void Start()
	{
	}

	protected virtual void Update()
	{
		Heal(DataChangePerSec(_healthPoint, _hpRecoverPerSec, _maxHealthPoint));
		foreach (var sys in _normalSystems)
		{
			sys.OnUpdate(this);
		}
	}

	protected virtual void FixedUpdate()
	{
		foreach (var sys in _physicalSystems)
		{
			sys.OnUpdate(this);
		}
	}

	/// <summary>
	/// 每秒数据变化，放在Update里的
	/// </summary>
	/// <param name="raw">原始数据</param>
	/// <param name="delta">每秒变化量</param>
	/// <param name="limit">数据最大值</param>
	/// <returns>变化后数据</returns>
	public static float DataChangePerSec(float raw, float delta, float limit)
	{
		var deltaTimeChange = delta * Time.deltaTime;
		return raw + deltaTimeChange > limit ? limit : raw + deltaTimeChange;
	}

	public void SetProperty<T>(T property) where T : class, IEntityProperty
	{
		//_properties.TryAdd(typeof(T), property);//草，没有.Net Standand 2.1的我要死了
		if (_properties.ContainsKey(typeof(T)))
		{
			_properties[typeof(T)] = property;
		}
		else
		{
			_properties.Add(typeof(T), property);
		}
	}

	public T GetProperty<T>() where T : class, IEntityProperty
	{
		if (_properties.TryGetValue(typeof(T), out var property))
		{
			return property as T;
		}

		throw new ArgumentException($"没有属性{typeof(T).FullName}");
	}

	public void AddSystem<T>() where T : IEntitySystem
	{
		if (EntitySystemManager.Instance.Systems.TryGetValue(typeof(T), out var sys))
		{
			if (sys.IsPhysic)
			{
				_physicalSystems.Add(sys);
			}
			else
			{
				_normalSystems.Add(sys);
			}
		}
		else
		{
			throw new ArgumentException($"没有系统{typeof(T)}");
		}
	}

	public void Heal(float amount)
	{
		var tryAdd = _healthPoint + amount;
		_healthPoint = tryAdd > _maxHealthPoint ? _maxHealthPoint : tryAdd;
	}
}
