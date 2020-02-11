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
        AddSkill(SkillFactory.Get<SkillRaceterShadowStrike>(Consts.SkillRaceterShadowStrike, this));
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        _swordResolve.OnUpdate();
        if (Input.GetKeyDown(KeyCode.W))
        {
            AddBuff(BuffFactory.GetDot(Consts.BUFF_Heal, this, 1f, 10, 1));
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            AddBuff(BuffFactory.GetState(Consts.BUFF_WindMark, this, 30, 1));
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            UseSkill(Consts.SkillRaceterShadowStrike.ToString());
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