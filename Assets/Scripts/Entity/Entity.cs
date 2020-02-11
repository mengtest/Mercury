using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public enum Face
{
    Left = -1,
    Right = 1,
    Up,
    Down
}

/// <summary>
/// 实体基类
/// </summary>
public abstract class Entity : MonoBehaviour
{
    /// <summary>
    /// 血量
    /// </summary>
    [SerializeField] protected float healthPoint;

    /// <summary>
    /// 最大血量
    /// </summary>
    [SerializeField] protected float maxHealthPoint;

    /// <summary>
    /// 每秒血量变化
    /// </summary>
    [SerializeField] protected float hpRecoverPerSec;

    /// <summary>
    /// 死亡后实体消失时间
    /// </summary>
    [SerializeField] protected float deadBodySurviveTime;

    /// <summary>
    /// 属性容器
    /// </summary>
    protected Dictionary<Type, IEntityProperty> properties;

    protected List<IEntitySystem> physicalSystems;
    protected List<IEntitySystem> normalSystems;

    protected Collider2D _collider;

    public float HealthPoint => healthPoint;
    public float MaxHealthPoint => maxHealthPoint;
    public float HpRecoverPerSec => hpRecoverPerSec;
    public float DeadBodySurviveTime => deadBodySurviveTime;
    public abstract EntityType EntityType { get; }
    public abstract AssetLocation RegisterName { get; }

    private void Awake() { OnAwake(); }

    protected virtual void OnAwake()
    {
        properties = new Dictionary<Type, IEntityProperty>();
        physicalSystems = new List<IEntitySystem>();
        normalSystems = new List<IEntitySystem>();
        _collider = GetComponent<Collider2D>();
        RegisterManager.OnEntityAwake(RegisterName, this);
    }

    private void Start() { OnStart(); }

    protected virtual void OnStart()
    {
        RegisterManager.OnEntityStart(RegisterName, this);
    }

    private void Update() { OnUpdate(); }

    protected virtual void OnUpdate()
    {
        var afterHeal = DataChangePerSec(healthPoint, hpRecoverPerSec, maxHealthPoint);
        healthPoint = afterHeal > maxHealthPoint ? maxHealthPoint : afterHeal;
        foreach (var sys in normalSystems)
        {
            sys.OnUpdate(this);
        }
    }

    private void FixedUpdate() { OnFixedUpdate(); }

    protected virtual void OnFixedUpdate()
    {
        foreach (var sys in physicalSystems)
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
        if (properties.ContainsKey(typeof(T)))
        {
            properties[typeof(T)] = property;
        }
        else
        {
            properties.Add(typeof(T), property);
        }
    }

    public T GetProperty<T>() where T : class, IEntityProperty
    {
        if (properties.TryGetValue(typeof(T), out var property))
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
                physicalSystems.Add(sys);
            }
            else
            {
                normalSystems.Add(sys);
            }
        }
        else
        {
            throw new ArgumentException($"没有系统{typeof(T)}");
        }
    }

    public void Heal(float amount)
    {
        var tryAdd = healthPoint + amount;
        healthPoint = tryAdd > maxHealthPoint ? maxHealthPoint : tryAdd;
    }

    public virtual bool IsGround(float distance)
    {
        var bound = _collider.bounds.extents;
        var left = Physics2D.Raycast(transform.position + new Vector3(bound.x, -bound.y - 0.01f, 0),
            Vector3.down,
            distance,
            LayerMask.GetMask("CrossableStep", "Default"));
        var right = Physics2D.Raycast(transform.position + new Vector3(-bound.x, -bound.y - 0.01f, 0),
            Vector3.down,
            distance,
            LayerMask.GetMask("CrossableStep", "Default"));
        return left.collider || right.collider;
    }

    [Obsolete]
    public bool IsGround(float distance, out Collider2D step)
    {
        var bound = _collider.bounds.extents;
        var left = Physics2D.Raycast(transform.position + new Vector3(bound.x, -bound.y - 0.01f, 0),
            Vector3.down,
            distance,
            LayerMask.GetMask("Step"));
        var right = Physics2D.Raycast(transform.position + new Vector3(-bound.x, -bound.y - 0.01f, 0),
            Vector3.down,
            distance,
            LayerMask.GetMask("Step"));
        var res = left.collider || right.collider;
        step = left.collider ? left.collider : right.collider;
        return res;
    }

    public Face GetFace()
    {
        var scale = transform.localScale;
        if (scale.x < 0)
        {
            return Face.Left;
        }

        if (scale.x > 0)
        {
            return Face.Right;
        }

        throw new ArgumentException();
    }

    public void Rotate(Face face)
    {
        var scale = transform.localScale;
        switch (face)
        {
            case Face.Left:
                scale.x = -math.abs(scale.x);
                break;
            case Face.Right:
                scale.x = math.abs(scale.x);
                break;
            default:
                return;
        }

        transform.localScale = scale;
    }
}