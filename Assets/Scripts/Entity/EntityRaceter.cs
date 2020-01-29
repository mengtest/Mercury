using UnityEngine;

/// <summary>
/// 南歌子
/// </summary>
public class EntityRaceter : EntityPlayer
{
    [SerializeField] private SwordResolve _swordResolve;

    protected override void Awake()
    {
        base.Awake();
        _swordResolve = new SwordResolve(this);
    }

    protected override void Start()
    {
        base.Start();
        SetProperty(_swordResolve);
        skills.AddSkill(new SkillRaceterShadowStrike(this));
        skills.AddSkill(new SkillRaceterBladeWave(this));
        skills.AddSkill(new SkillRaceterHasaki(this));
    }

    protected override void Update()
    {
        base.Update();
        _swordResolve.OnUpdate();
        if (Input.GetKeyDown(KeyCode.K))
        {
            UseSkill(typeof(SkillRaceterShadowStrike));
        }
    }

    public override Damage DealDamage(float coe, DamageType damageType, Entity target)
    {
        Damage dmg;
        if (!_swordResolve.swordState && _swordResolve.IsResolveFull)
        {
            var normalDmg = DamageCalculator.GetDamage(coe, damageType);
            var critDmg = DamageCalculator.GetCritDamage(normalDmg, 1, damageType);
            dmg = new Damage(this,
                normalDmg,
                critDmg,
                damageType);
            EventOnAttackTarget(ref dmg, target);
            return dmg;
        }

        dmg = new Damage(this,
            DamageCalculator.GetFinalDamage(coe, damageType, out var extraCritDamage),
            extraCritDamage,
            damageType);
        EventOnAttackTarget(ref dmg, target);
        return dmg;
    }
}