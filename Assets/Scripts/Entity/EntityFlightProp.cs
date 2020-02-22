using System;
using UnityEngine;

/// <summary>
/// 飞行道具
/// </summary>
[RequireComponent(typeof(Collider2D))]
[EventSubscriber]
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

    protected override void OnAwake()
    {
        _collider = GetComponent<Collider2D>();
        EventManager.Instance.Publish(this, new EntityEvent.Awake(this));
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

    [Subscribe]
    private static void OnRegister(object sender, RegisterEvent.AfterAuto e)
    {
        e.manager.Register(EntityEntry.Create()
            .SetRegisterName(Consts.EntityFlightProp)
            .Build());
    }
}