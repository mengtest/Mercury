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
        //Debug.Log($"暴击率:{DamageCalculator.CritProbability}\t全伤害收益:{DamageCalculator.DamageUpgrade[2]}");
        if (Input.GetKeyDown(KeyCode.K))
        {
            UseSkill(typeof(SkillRaceterBladeWave));
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