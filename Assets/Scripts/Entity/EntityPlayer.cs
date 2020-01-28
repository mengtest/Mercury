using System;
using UnityEngine;

/// <summary>
/// 玩家
/// </summary>
public class EntityPlayer : Entity, IAttackable, IBuffable, ISkillable
{
    [SerializeField] private BasicCapability _basicCapability = new BasicCapability();
    [SerializeField] private ElementAffinity _elementAffinity = new ElementAffinity();
    [SerializeField] private MoveCapability _moveCapability = new MoveCapability();

    protected BuffWrapper buffs;
    protected SkillWrapper skills;

    public override EntityType EntityType { get; } = EntityType.Player;

    protected override void Start()
    {
        base.Start();
        SetProperty(_basicCapability);
        SetProperty(_elementAffinity);
        SetProperty(_moveCapability);

        AddSystem<MoveSystem>();
        DamageCalculator = new DamageChainCalculator(this);
        buffs = new BuffWrapper(this);
        skills = new SkillWrapper(this, new NormalState(this));
        skills.AddSkill(new StiffnessState(this));
    }

    protected override void Update()
    {
        base.Update();
        buffs.OnUpdate();
        skills.OnUpdate();
    }

    #region IBuffable

    public void AddBuff(IBuff buff) { buffs.AddBuff(buff); }

    public IBuff GetBuff(Type buffType, BuffVariant variant) { return buffs.GetBuff(buffType, variant); }

    public bool RemoveBuff(Type buffType, BuffVariant variant) { return buffs.RemoveBuff(buffType, variant); }

    public bool TryGetBuff(Type buffType, BuffVariant variant, out IBuff buff)
    {
        return buffs.TryGetBuff(buffType, variant, out buff);
    }

    public bool HasBuff(Type buffType, BuffVariant variant) { return buffs.HasBuff(buffType, variant); }

    #endregion

    #region IAttackable

    public float PhysicsAttack => _basicCapability.phyAttack;
    public float MagicAttack => _basicCapability.magAttack;
    public int Crit => _basicCapability.criticalPoint;
    public DamageChainCalculator DamageCalculator { get; private set; }

    public event Action<Damage, Entity> OnAttackTarget;

    public virtual Damage DealDamage(float coe, DamageType damageType, Entity target)
    {
        var dmg = new Damage(this, DamageCalculator.GetFinalDamage(coe, damageType, out var c), c, damageType);
        EventOnAttackTarget(ref dmg, target);
        return dmg;
    }

    protected void EventOnAttackTarget(ref Damage dmg, Entity target) { OnAttackTarget?.Invoke(dmg, target); }

    public virtual void UnderAttack(in Damage damage)
    {
        _healthPoint -= DamageUtility.ReduceDmgFormula(damage.value, _basicCapability, damage.type);
    }

    #endregion

    #region ISkillable

    public FSMSystem Skills => skills.FSMSystem;

    public void AddSkill(AbstractSkill skill) { skills.AddSkill(skill); }

    public bool RemoveSkill(Type skillType) { return skills.RemoveSkill(skillType); }

    public void UseSkill(Type skillType) { skills.UseSkill(skillType); }

    public void UseSkill<T>(out T skill) where T : AbstractSkill { skills.UseSkill(out skill); }

    public void OnUpdate() { skills.OnUpdate(); }

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