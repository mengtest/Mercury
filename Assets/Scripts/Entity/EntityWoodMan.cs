using System;

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
    public int Crit { get; } = 0;
    public DamageChainCalculator DamageCalculator { get; }

    public event Action<Damage, Entity> OnAttackTarget;

    public Damage DealDamage(float coe, DamageType damage, Entity target)
    {
        var dmg = new Damage(this, 0, 0, damage);
        OnAttackTarget?.Invoke(dmg, target);
        return dmg;
    }

    public void UnderAttack(in Damage damage)
    {
        var dmg = damage.FinalDamage;
        _healthPoint -= dmg;
        UIManager.Instance.ShowDamage(transform, (int) dmg, damage.type);
    }
}