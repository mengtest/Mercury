using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 南歌子
/// </summary>
public class EntityRaceter : EntityPlayer
{
    [SerializeField] private SwordResolve _swordResolve;
    public GameObject SkillObjCollection { get; private set; }
    public HashSet<Entity> HasWindMarkBuff { get; } = new HashSet<Entity>();

    protected override void OnAwake()
    {
        base.OnAwake();
        _swordResolve = new SwordResolve(this);
        SkillObjCollection = new GameObject("EntityRaceterSkills");
    }

    protected override void OnStart()
    {
        base.OnStart();
        SetProperty(_swordResolve);
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

        if (Input.GetKeyDown(KeyCode.J))
        {
            UseSkill<SkillRaceterIaiAndSwallowFlip>();
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            AddBuff(BuffFactory.GetDot(Consts.BUFF_Heal, this, 1f, 10, 1));
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            AddBuff(BuffFactory.GetState(Consts.BUFF_WindMark, this, 30, 1));
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            UseSkill<SkillRaceterFlashCut>();
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