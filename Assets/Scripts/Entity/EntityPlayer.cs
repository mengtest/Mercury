using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// 玩家
/// </summary>
public class EntityPlayer : Entity, IAttackable, IBuffable, ISkillable
{
    [SerializeField] private BasicCapability _basicCapability = new BasicCapability();
    [SerializeField] private ElementAffinity _elementAffinity = new ElementAffinity();
    [SerializeField] private MoveCapability _moveCapability = new MoveCapability();
    [SerializeField] private SwordResolve _swordResolve = new SwordResolve();

    private BuffWrapper _buffs;
    private SkillWrapper _skills;

    public FSMSystem Skills => _skills.FSMSystem;

    public override EntityType EntityType { get; } = EntityType.Player;

    protected override void Start()
    {
        base.Start();
        SetProperty(_basicCapability);
        SetProperty(_elementAffinity);
        SetProperty(_moveCapability);
        SetProperty(_swordResolve);

        AddSystem<MoveSystem>();
        DamageCalculators = new DamageChain(this);
        DamageCalculators.Add(new DamageCalculator(DamageIncome.Subjoin, this, 0.1f, DamageType.Physics));
        DamageCalculators.Add(new DamageCalculator(DamageIncome.Subjoin, this, 0.2f, DamageType.Physics));

        DamageCalculators.Add(new DamageCalculator(DamageIncome.Upgrade, this, 0.2f, DamageType.Physics));

        _buffs = new BuffWrapper(this);
        _skills = new SkillWrapper(this, new NormalState(this));
        _skills.AddSkill(new StiffnessState(this));
        _skills.AddSkill(new SkillRaceterShadowStrike(this));
        _skills.AddSkill(new SkillRaceterBladeWave(this));
        _skills.AddSkill(new SkillRaceterHasaki(this));
    }

    protected override void Update()
    {
        base.Update();
        if (Input.GetKeyDown(KeyCode.K))
        {
            UseSkill(typeof(SkillRaceterShadowStrike));
        }

        _buffs.OnUpdate();
        _skills.OnUpdate();
    }

    #region IBuffable

    public void AddBuff(IBuff buff) { _buffs.AddBuff(buff); }

    public IBuff GetBuff(Type buffType, BuffVariant variant) { return _buffs.GetBuff(buffType, variant); }

    public bool RemoveBuff(Type buffType, BuffVariant variant) { return _buffs.RemoveBuff(buffType, variant); }

    public bool TryGetBuff(Type buffType, BuffVariant variant, out IBuff buff)
    {
        return _buffs.TryGetBuff(buffType, variant, out buff);
    }

    public bool HasBuff(Type buffType, BuffVariant variant) { return _buffs.HasBuff(buffType, variant); }

    #endregion

    #region IAttackable

    public float PhysicsAttack => _basicCapability.phyAttack;
    public float MagicAttack => _basicCapability.magAttack;
    public DamageChain DamageCalculators { get; private set; }

    public Damage DealDamage(float coe, DamageType damageType)
    {
        return new Damage(this, DamageCalculators.Calculate(coe, damageType), damageType);
    }

    public void UnderAttack(in Damage damage)
    {
        _healthPoint -= DamageUtility.ReduceDmgFormula(damage.value, _basicCapability, damage.type);
    }

    #endregion

    #region ISkillable

    public void AddSkill(AbstractSkill skill) { _skills.AddSkill(skill); }

    public bool RemoveSkill(Type skillType) { return _skills.RemoveSkill(skillType); }

    public void UseSkill(Type skillType) { _skills.UseSkill(skillType); }

    public void UseSkill<T>(out T skill) where T : AbstractSkill { _skills.UseSkill(out skill); }

    public void OnUpdate() { _skills.OnUpdate(); }

    #endregion

    private void OnTriggerEnter2D(Collider2D other) { UpTriggerStep(other); }

    private void OnTriggerStay2D(Collider2D other) { UpTriggerStep(other); }

    private void UpTriggerStep(Collider2D other)
    {
        if (other.CompareTag(Consts.TAG_StepCross))
        {
            Physics2D.IgnoreCollision(_collider, other, true);
        }
    }
}