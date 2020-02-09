using System;
using System.Collections.Generic;
using Prime31;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AddressableAssets;

/// <summary>
/// 玩家
/// </summary>
public class EntityPlayer : Entity, IAttackable, IBuffable, ISkillable, IMoveable
{
    [SerializeField] private BasicCapability _basicCapability = new BasicCapability();
    [SerializeField] private ElementAffinity _elementAffinity = new ElementAffinity();
    [SerializeField] private MoveCapability _moveCapability = new MoveCapability();

    protected BuffHandler buffs;
    protected CharacterController2D controller;

    public override EntityType EntityType { get; } = EntityType.Player;

    protected override void OnStart()
    {
        base.OnStart();
        controller = GetComponent<CharacterController2D>();
        SetProperty(_basicCapability);
        SetProperty(_elementAffinity);
        SetProperty(_moveCapability);

        DamageCalculator = new DamageChainCalculator(this);
        buffs = new BuffHandler(this);
        SkillFsmSystem = new FSMSystem(new NormalState(this));
        AddSkill(new StiffnessState(this));
        // foreach (var skill in SkillObjects)
        // {
        //     var obj = await skill.InstantiateAsync(transform, true).Task;
        //     var skillObj = obj.GetComponent<SkillObject>();
        //     if (skillObj)
        //     {
        //         var fsmState = skillObj as IFSMState;
        //         AddSkill(fsmState);
        //         fsmState.Init();
        //     }
        //     else
        //     {
        //         throw new ArgumentException();
        //     }
        // }
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
                    _moveCapability.velocity.y = math.sqrt(2f * _moveCapability.jumpHeight * -_moveCapability.gravity);
                }
            }

            var smoothedMovementFactor =
                controller.isGrounded ? _moveCapability.groundDamping : _moveCapability.inAirDamping;
            _moveCapability.velocity.x = math.lerp(_moveCapability.velocity.x,
                normalizedHorizontalSpeed * _moveCapability.runSpeed,
                Time.deltaTime * smoothedMovementFactor);
            _moveCapability.velocity.y += _moveCapability.gravity * Time.deltaTime;
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
    public void AddBuff(BuffFlyweightDot dot) { buffs.Add(dot); }
    public void AddBuff(BuffFlyweightState state) { buffs.Add(state); }
    public bool RemoveDotBuff(string buffName) { return buffs.RemoveDot(buffName); }
    public bool RemoveStateBuff(string buffName) { return buffs.RemoveState(buffName); }
    public BuffFlyweightDot GetDotBuff(string buffName) { return buffs.GetDot(buffName); }
    public BuffFlyweightState GetStateBuff(string buffName) { return buffs.GetState(buffName); }
    public bool ContainsDotBuff(string buffName) { return buffs.ContainsDot(buffName); }
    public bool ContainsStateBuff(string buffName) { return buffs.ContainsState(buffName); }
    public bool TryGetDotBuff(string buffName, out BuffFlyweightDot dot) { return buffs.TryGetDot(buffName, out dot); }

    public bool TryGetStateBuff(string buffName, out BuffFlyweightState state)
    {
        return buffs.TryGetState(buffName, out state);
    }

    #endregion

    #region IAttackable

    public float PhysicsAttack => _basicCapability.phyAttack;
    public float MagicAttack => _basicCapability.magAttack;
    public int Crit => _basicCapability.criticalPoint;
    public DamageChainCalculator DamageCalculator { get; private set; }

    public event Action<Damage, IAttackable> OnAttackTarget;

    public virtual Damage CalculateDamage(float coe, DamageType damage)
    {
        return new Damage(this, DamageCalculator.GetFinalDamage(coe, damage, out var c), c, damage);
    }

    public Damage DealDamage(in Damage damage, IAttackable target)
    {
        OnAttackTarget?.Invoke(damage, target);
        return damage;
    }

    public virtual void UnderAttack(in Damage damage)
    {
        UIManager.Instance.ShowDamage(transform, damage.FinalDamage, damage.type);
        healthPoint -= DamageUtility.ReduceDmgFormula(damage.value, _basicCapability, damage.type);
    }

    #endregion

    #region ISkillable

    public FSMSystem SkillFsmSystem { get; private set; }

    public void AddSkill(IFSMState skill) { SkillFsmSystem.AddState(skill); }

    public bool RemoveSkill<T>() where T : class, IFSMState { return SkillFsmSystem.RemoveState(typeof(T)); }

    public void UseSkill<T>() where T : class, IFSMState { SkillFsmSystem.SwitchState(typeof(T)); }

    public void UseSkill<T>(out T skill) where T : class, IFSMState { SkillFsmSystem.SwitchState(out skill); }

    public void OnUpdateSkills() { SkillFsmSystem.OnUpdate(); }

    #endregion

    #region IMoveable

    public float MoveSpeed { get => _moveCapability.runSpeed; set => _moveCapability.runSpeed = value; }
    public float JumpSpeed { get => _moveCapability.jumpHeight; set => _moveCapability.jumpHeight = value; }
    public float GroundDamping { get => _moveCapability.groundDamping; set => _moveCapability.groundDamping = value; }
    public float AirDamping { get => _moveCapability.inAirDamping; set => _moveCapability.inAirDamping = value; }
    public float Gravity { get => _moveCapability.gravity; set => _moveCapability.gravity = value; }
    public Vector2 Velocity { get => _moveCapability.velocity; set => _moveCapability.velocity = value; }

    public void Move(Vector2 velocity)
    {
        controller.move(velocity);
        _moveCapability.velocity = controller.velocity;
    }

    #endregion
}