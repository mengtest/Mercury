using System;
using UnityEngine;

/// <summary>
/// 飞行道具
/// </summary>
public class EntityFlightProp : Entity
{
    public float maxLiveTime;

    private float _liveTime;

    public event Action<EntityFlightProp> OnUpdateAction;
    public event Func<EntityFlightProp, bool> IsDead;
    public event Action<EntityFlightProp> OnDead;
    
    public Collider2D Trigger { get; private set; }

    public override EntityType EntityType { get; } = EntityType.Flight;

    protected override void OnAwake() { _collider = GetComponent<Collider2D>(); }

    protected override void OnUpdate()
    {
        if (_liveTime > 0)
        {
            _liveTime -= Time.deltaTime;
            OnUpdateAction?.Invoke(this);
            var res = IsDead?.Invoke(this);
            if (res.HasValue && res.Value)
            {
                OnDead(this);
            }
        }
        else
        {
            OnDead(this);
        }
    }

    protected override void OnFixedUpdate() { }

    private void OnTriggerEnter2D(Collider2D other) { Trigger = other; }

    private void OnTriggerStay2D(Collider2D other) { Trigger = other; }

    private void OnTriggerExit2D(Collider2D other) { Trigger = null; }

    public void Reset()
    {
        OnUpdateAction = null;
        OnDead = null;
        IsDead = null;
        _liveTime = maxLiveTime;
        Trigger = null;
    }
}