using System.Collections.Generic;

public class EntityWoodMan : Entity, IAttackable
{
    public override EntityType EntityType { get; } = EntityType.Enemy;

    protected override void Start()
    {
        base.Start();
        _healthPoint = 100_000;
        _maxHealthPoint = 100_000;
        _hpRecoverPerSec = 100;
    }

    public float PhysicsAttack { get; } = 0;
    public float MagicAttack { get; } = 0;
    public DamageChain DamageCalculators { get; }

    public Damage DealDamage(float coefficient, DamageType damageType) { return new Damage(this, 0, damageType); }

    public void UnderAttack(in Damage damage)
    {
        _healthPoint -= damage.value;
        UIManager.Instance.ShowDamage(transform, (int) damage.value, damage.type);
    }
}