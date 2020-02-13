using System;
using UnityEngine;

/// <summary>
/// 飞行道具
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class EntityFlightProp : Entity
{
    public float maxLivingTime = 10;
    private float _deathTime;

    public event Action<EntityFlightProp> OnUpdateAction;
    public event Func<EntityFlightProp, bool> IsDead;
    public event Action<EntityFlightProp> OnDead;
    public event Action<Collider2D, EntityFlightProp> OnTriggerEnter;
    public event Action<Collider2D, EntityFlightProp> OnTriggerStay;
    public event Action<Collider2D, EntityFlightProp> OnTriggerExit;

    public override EntityType EntityType { get; } = EntityType.Flight;

    public override AssetLocation RegisterName { get; } = Consts.EntityFlightProp;

    protected override void OnAwake()
    {
        RegisterManager.OnEntityAwake(RegisterName, this);
        _collider = GetComponent<Collider2D>();
    }

    protected override void OnUpdate()
    {
        if (Time.time >= _deathTime)
        {
            OnDead?.Invoke(this);
        }
        else
        {
            OnUpdateAction?.Invoke(this);
            var res = IsDead?.Invoke(this);
            if (res.HasValue && res.Value)
            {
                OnDead?.Invoke(this);
            }
        }
    }

    protected override void OnFixedUpdate() { }

    private void OnEnable() { _deathTime = Time.time + maxLivingTime; }

    private void OnTriggerEnter2D(Collider2D other) { OnTriggerEnter?.Invoke(other, this); }

    private void OnTriggerStay2D(Collider2D other) { OnTriggerStay?.Invoke(other, this); }

    private void OnTriggerExit2D(Collider2D other) { OnTriggerExit?.Invoke(other, this); }

    public void Clear()
    {
        OnUpdateAction = null;
        IsDead = null;
        OnDead = null;
        OnTriggerEnter = null;
        OnTriggerStay = null;
        OnTriggerExit = null;
    }
}