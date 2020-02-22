using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 南歌子
/// </summary>
[EventSubscriber]
public class EntityRaceter : EntityPlayer
{
    [SerializeField] private SwordResolve _swordResolve;

    public HashSet<Entity> HasWindMarkBuff { get; } = new HashSet<Entity>();
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
        SetProperty(_swordResolve);
        base.OnStart();
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
            UseSkill(Consts.SkillRaceterShadowStrike);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            UseSkill(Consts.SkillRaceterIaiAndSwallowFlip);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            UseSkill(Consts.SkillRaceterBladeWave);
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            UseSkill(Consts.SkillRaceterFlashCut);
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            UseSkill(Consts.SkillRaceterWindPace);
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            UseSkill(Consts.SkillRaceterMoonAttack);
        }
    }

    public override Damage CalculateDamage(float coe, DamageType type)
    {
        if (!_swordResolve.swordState && _swordResolve.IsResolveFull)
        {
            var dmg = DamageCalculator.GetDamage(coe, type);
            var crit = type == DamageType.True
                ? 0
                : DamageCalculator.GetCritDamage(dmg, DamageCalculator.CritCoe.Multiply, 1);
            return new Damage(this, dmg, crit, type);
        }

        return DamageCalculator.SimpleDamage(coe, type);
    }

    [Subscribe]
    private static void OnRegisterEntity(object sender, RegisterEvent.AfterAuto e)
    {
        e.manager.Register(EntityEntry.Create()
            .SetRegisterName(Consts.EntityRaceter)
            .AddDependEntry(Consts.SkillRaceterBladeWave)
            .AddDependEntry(Consts.SkillRaceterFlashCut)
            .AddDependEntry(Consts.SkillRaceterWindPace)
            .AddDependEntry(Consts.SkillRaceterShadowStrike)
            .AddDependEntry(Consts.SkillRaceterIaiAndSwallowFlip)
            .Build());
    }
}