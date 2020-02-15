using System;
using Prime31;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// 玩家
/// </summary>
public abstract class EntityPlayer : Entity, IAttackable, IBuffable, ISkillable, IMovable
{
    [SerializeField] private BasicCapability _basicCapability = new BasicCapability();
    [SerializeField] private ElementAffinity _elementAffinity = new ElementAffinity();
    [SerializeField] private MoveCapability _moveCapability = new MoveCapability();

    protected BuffHandler buffs;
    protected CharacterController2D controller;

    public override EntityType EntityType { get; } = EntityType.Player;

    protected override void OnStart()
    {
        controller = GetComponent<CharacterController2D>();
        SetProperty(_basicCapability);
        SetProperty(_elementAffinity);
        SetProperty(_moveCapability);

        DamageCalculator = new DamageCalculator(this);
        buffs = new BuffHandler(this);
        SkillFsmSystem = new FSMSystem();
        AddSkill(EntityUtility.GetSkill<NormalState>(Consts.SkillNormal, this));
        AddSkill(EntityUtility.GetSkill<StiffnessState>(Consts.SkillStiffness, this));
        UseSkill(Consts.SkillNormal);
        MotionCalculator = new MotionCalculator(this);
        base.OnStart();
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        OnUpdateBuffs();
        OnUpdateSkills();

        if (_moveCapability.canMove)
        {
            if (controller.isGrounded)
            {
                _moveCapability.velocity.y = 0;
                _moveCapability.RecoverJumpCount();
            }
            else
            {
                _moveCapability.UpdateJumpCD();
            }

            float normalizedHorizontalSpeed;
            if (Input.GetKey(KeyCode.RightArrow))
            {
                normalizedHorizontalSpeed = 1;
                Rotate(Face.Right);
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                normalizedHorizontalSpeed = -1;
                Rotate(Face.Left);
            }
            else
            {
                normalizedHorizontalSpeed = 0;
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (_moveCapability.TryJump())
                {
                    _moveCapability.velocity.y = math.sqrt(2f * MotionCalculator.JumpSpeed.Add * -MotionCalculator.Gravity.Add);
                }
            }

            var smoothedMovementFactor = controller.isGrounded
                ? MotionCalculator.GroundDamping.Add
                : MotionCalculator.AirDamping.Add;
            _moveCapability.velocity.x = math.lerp(_moveCapability.velocity.x,
                normalizedHorizontalSpeed * MotionCalculator.MoveSpeed.Add,
                Time.deltaTime * smoothedMovementFactor);
            _moveCapability.velocity.y += MotionCalculator.Gravity.Add * Time.deltaTime;
            if (controller.isGrounded && Input.GetKey(KeyCode.DownArrow))
            {
                _moveCapability.velocity.y *= 11f;
                controller.ignoreOneWayPlatformsThisFrame = true;
            }

            Move(_moveCapability.velocity * Time.deltaTime);
        }
    }

    public override bool IsGround(float distance) { return controller.isGrounded; }

    #region IBuffable

    public void OnUpdateBuffs() { buffs.OnUpdate(); }
    public void AddBuff(BuffStack buff) { buffs.Add(buff); }
    public bool RemoveBuff(AssetLocation location) { return buffs.Remove(location.ToString()); }
    public bool HasBuff(AssetLocation location) { return buffs.Contains(location.ToString()); }
    public BuffStack GetBuff(AssetLocation location) { return buffs[location.ToString()]; }

    public bool TryGetBuff(AssetLocation location, out BuffStack buff) { return buffs.TryGet(location.ToString(), out buff); }

    #endregion

    #region IAttackable

    public float PhysicsAttack => _basicCapability.phyAttack;
    public float MagicAttack => _basicCapability.magAttack;
    public float CritCoefficient => _basicCapability.criticalCoePoint;
    public int CritProbability => _basicCapability.criticalPoint;
    public DamageCalculator DamageCalculator { get; private set; }

    public event Action<Damage, IAttackable> OnAttackTarget;

    public virtual Damage CalculateDamage(float coe, DamageType type) { return DamageCalculator.SimpleDamage(coe, type); }

    public Damage DealDamage(in Damage damage, IAttackable target)
    {
        OnAttackTarget?.Invoke(damage, target);
        return damage;
    }

    public virtual void UnderAttack(in Damage damage)
    {
        UIManager.Instance.ShowDamage(transform, damage.FinalDamage, damage.type);
        healthPoint -= CalculateUtility.ReduceDmgFormula(damage.value, _basicCapability, damage.type);
    }

    #endregion

    #region ISkillable

    public FSMSystem SkillFsmSystem { get; private set; }
    public abstract GameObject SkillCollection { get; protected set; }

    public void AddSkill(IFSMState skill) { SkillFsmSystem.AddState(skill); }
    public bool RemoveSkill(AssetLocation location) { return SkillFsmSystem.RemoveState(location.ToString()); }
    public void UseSkill(AssetLocation location) { SkillFsmSystem.SwitchState(location.ToString()); }

    public void UseSkill(AssetLocation location, out IFSMState skill) { SkillFsmSystem.SwitchState(location.ToString(), out skill); }

    public void OnUpdateSkills() { SkillFsmSystem.OnUpdate(); }

    #endregion

    #region IMoveable

    public float MoveSpeed { get => _moveCapability.runSpeed; set => _moveCapability.runSpeed = value; }
    public float JumpSpeed { get => _moveCapability.jumpHeight; set => _moveCapability.jumpHeight = value; }
    public float GroundDamping { get => _moveCapability.groundDamping; set => _moveCapability.groundDamping = value; }
    public float AirDamping { get => _moveCapability.inAirDamping; set => _moveCapability.inAirDamping = value; }
    public float Gravity { get => _moveCapability.gravity; set => _moveCapability.gravity = value; }
    public Vector2 Velocity { get => _moveCapability.velocity; set => _moveCapability.velocity = value; }

    public MotionCalculator MotionCalculator { get; private set; }

    public void Move(Vector2 velocity)
    {
        controller.move(velocity);
        _moveCapability.velocity = controller.velocity;
    }

    #endregion
}