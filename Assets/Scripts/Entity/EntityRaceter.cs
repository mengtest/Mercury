using UnityEngine;

/// <summary>
/// 南歌子
/// </summary>
public class EntityRaceter : EntityPlayer
{
    [SerializeField] private SwordResolve _swordResolve;

    protected override void OnAwake()
    {
        base.OnAwake();
        _swordResolve = new SwordResolve(this);
    }

    protected override void OnStart()
    {
        base.OnStart();
        SetProperty(_swordResolve);
        AddSkill(new SkillRaceterBladeWave(this));
        AddSkill(new SkillRaceterHasaki(this));
        AddSkill(new SkillRaceterIaiAndSwallowFlip(this));
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        _swordResolve.OnUpdate();
        if (Input.GetKeyDown(KeyCode.K))
        {
            UseSkill<SkillRaceterBladeWave>();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            UseSkill<SkillRaceterShadowStrike>();
        }
    }

    public override Damage CalculateDamage(float coe, DamageType damage)
    {
        Damage dmg;
        if (!_swordResolve.swordState && _swordResolve.IsResolveFull)
        {
            var normalDmg = DamageCalculator.GetDamage(coe, damage);
            var critDmg = DamageCalculator.GetCritDamage(normalDmg, 1, damage);
            dmg = new Damage(this,
                normalDmg,
                critDmg,
                damage);
        }
        else
        {
            dmg = new Damage(this,
                DamageCalculator.GetFinalDamage(coe, damage, out var extraCritDamage),
                extraCritDamage,
                damage);
        }

        return dmg;
    }
}