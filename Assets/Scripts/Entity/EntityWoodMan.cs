using System;

[EventSubscriber]
public class EntityWoodMan : Entity, IAttackable
{
    public override EntityType EntityType { get; } = EntityType.Enemy;
    // public override AssetLocation RegisterName { get; } = Consts.EntityWoodMan;

    protected override void OnStart()
    {
        base.OnStart();
        healthPoint = 100_000;
        maxHealthPoint = 100_000;
        hpRecoverPerSec = 100;
    }

    public float PhysicsAttack { get; } = 0;
    public float MagicAttack { get; } = 0;
    public float CritCoefficient { get; } = 0;
    public int CritProbability { get; } = 0;
    public DamageCalculator DamageCalculator { get; }

    public event Action<Damage, IAttackable> OnAttackTarget;

    public Damage CalculateDamage(float coe, DamageType type) { return new Damage(this, 0, 0, DamageType.Physics); }

    public Damage DealDamage(in Damage damage, IAttackable target)
    {
        OnAttackTarget?.Invoke(damage, target);
        return damage;
    }

    public void UnderAttack(in Damage damage)
    {
        var dmg = damage.FinalDamage;
        healthPoint -= dmg;
        UIManager.Instance.ShowDamage(transform, (int) dmg, damage.type);
    }

    [Subscribe]
    private static void OnRegister(object sender, RegisterEvent.AfterAuto e)
    {
        e.manager.Register(EntityEntry.Create()
            .SetRegisterName(Consts.EntityWoodMan)
            .Build());
    }
}