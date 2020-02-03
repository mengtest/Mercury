﻿using System;
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

    protected BuffWrapper buffs;
    //protected SkillWrapper skills;

    protected CharacterController2D controller;

    public override EntityType EntityType { get; } = EntityType.Player;

    protected override async void OnStart()
    {
        base.OnStart();
        controller = GetComponent<CharacterController2D>();
        SetProperty(_basicCapability);
        SetProperty(_elementAffinity);
        SetProperty(_moveCapability);

        DamageCalculator = new DamageChainCalculator(this);
        buffs = new BuffWrapper(this);
        _skillFsmSystem = new FSMSystem(new NormalState(this));
        AddSkill(new StiffnessState(this));
        foreach (var skill in SkillObjects)
        {
            var obj = await skill.InstantiateAsync(transform, true).Task;
            var skillObj = obj.GetComponent<SkillObject>();
            if (skillObj)
            {
                var fsmState = skillObj as IFSMState;
                AddSkill(fsmState);
                fsmState.Init();
            }
        }
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        buffs.OnUpdate();
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
            if (controller.isGrounded && Input.GetKey(KeyCode.DownArrow) && _moveCapability.canMove)
            {
                _moveCapability.velocity.y *= 3f;
                transform.position -= new Vector3(0, 0.025f, 0);
                controller.ignoreOneWayPlatformsThisFrame = true;
            }

            Move(_moveCapability.velocity * Time.deltaTime);
        }
    }

    public override bool IsGround(float distance) { return controller.isGrounded; }

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
        healthPoint -= DamageUtility.ReduceDmgFormula(damage.value, _basicCapability, damage.type);
    }

    #endregion

    #region ISkillable

    [SerializeField] private List<AssetReference> _skillObjects = new List<AssetReference>();

    private FSMSystem _skillFsmSystem;

    public FSMSystem SkillFsmSystem => _skillFsmSystem;
    public List<AssetReference> SkillObjects => _skillObjects;

    public void AddSkill(IFSMState skill) { _skillFsmSystem.AddState(skill); }

    public bool RemoveSkill<T>() where T : class, IFSMState { return _skillFsmSystem.RemoveState(typeof(T)); }

    public void UseSkill<T>() where T : class, IFSMState { _skillFsmSystem.SwitchState(typeof(T)); }

    public void UseSkill<T>(out T skill) where T : class, IFSMState { _skillFsmSystem.SwitchState(out skill); }

    public void OnUpdateSkills() { _skillFsmSystem.OnUpdate(); }

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