using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 南歌子
/// </summary>
public class EntityRaceter : EntityPlayer
{
    [SerializeField] private SwordResolve _swordResolve;
    public HashSet<Entity> HasWindMarkBuff { get; } = new HashSet<Entity>();
    public override AssetLocation RegisterName { get; } = Consts.EntityRaceter;
    public override GameObject SkillCollection { get; protected set; }

    protected override void OnAwake()
    {
        base.OnAwake();
        _swordResolve = new SwordResolve(this);
        SkillCollection = new GameObject("entity.raceter_skills");
        SkillCollection.transform.position = new Vector3(0, 0, -1);
    }

    protected override void OnStart()
    {
        base.OnStart();
        SetProperty(_swordResolve);
        AddSkill(EntityUtility.GetSkill<SkillRaceterShadowStrike>(Consts.SkillRaceterShadowStrike, this));
        AddSkill(EntityUtility.GetSkill<SkillRaceterIaiAndSwallowFlip>(Consts.SkillRaceterIaiAndSwallowFlip, this));
        AddSkill(EntityUtility.GetSkill<SkillRaceterBladeWave>(Consts.SkillRaceterBladeWave, this));
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        _swordResolve.OnUpdate();
        if (Input.GetKeyDown(KeyCode.L))
        {
            AddBuff(EntityUtility.GetBuffDot(Consts.BuffHeal, this, 1f, -1, 1));
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            AddBuff(EntityUtility.GetBuffState(Consts.BuffWindMark, this, 30, 1));
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            UseSkill(Consts.SkillRaceterShadowStrike.ToString());
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            UseSkill(Consts.SkillRaceterIaiAndSwallowFlip.ToString());
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            UseSkill(Consts.SkillRaceterBladeWave.ToString());
        }
    }

    public override Damage CalculateDamage(float coe, DamageType damage)
    {
        Damage dmg;
        if (!_swordResolve.swordState && _swordResolve.IsResolveFull)
        {
            var normalDmg = DamageCalculator.GetDamage(coe, damage);
            float critDmg;
            if (damage == DamageType.True)
            {
                critDmg = 0;
            }
            else
            {
                critDmg = DamageCalculator.GetCritDamage(normalDmg, 1, damage);
            }

            dmg = new Damage(this,
                normalDmg,
                critDmg,
                damage);
        }
        else
        {
            var v = DamageCalculator.GetFinalDamage(coe, damage, out var extraCritDamage);
            if (damage == DamageType.True)
            {
                extraCritDamage = 0;
            }

            dmg = new Damage(this,
                v,
                extraCritDamage,
                damage);
        }

        return dmg;
    }
}