using System;
using UnityEngine;

/// <summary>
/// 飞行道具
/// </summary>
public class EntityFlightProp : Entity
{
    public float maxLiveTime;
    public Action<EntityFlightProp> onUpdate;
    public Func<EntityFlightProp, bool> isDead;
    public Action<EntityFlightProp> onDead;

    private float liveTime;
    
    public Collider2D Trigger { get; private set; }
    
    protected override void Awake() { _collider = GetComponent<Collider2D>(); }

    protected override void Update()
    {
        if (liveTime > 0)
        {
            liveTime -= Time.deltaTime;
            onUpdate?.Invoke(this);
            var res = isDead?.Invoke(this);
            if (res.HasValue && res.Value)
            {
                onDead(this);
            }
        }
        else
        {
            onDead(this);
        }
    }

    protected override void FixedUpdate() { }

    private void OnTriggerEnter2D(Collider2D other) { Trigger = other; }

    private void OnTriggerStay2D(Collider2D other) { Trigger = other; }

    private void OnTriggerExit2D(Collider2D other) { Trigger = null; }

    public void Reset()
    {
        onUpdate = null;
        onDead = null;
        isDead = null;
        liveTime = maxLiveTime;
        Trigger = null;
    }
}