using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// 居合·燕返
/// </summary>
public class SkillRaceterIaiAndSwallowFlip : SkillObject //TODO:居合等特效好了再做
{
    public Animator swallowFlipAnim;
    public EntityRaceter raceter;
    private TriggerEventCallback _swallowTrigger;
    private MoveCapability _playerMove;
    private SwordResolve _swordResolve;
    private float _swallowFlipLength;
    private Damage _damage;
    private float _animDuration;
    private readonly Dictionary<Collider2D, (int, float)> _attacked = new Dictionary<Collider2D, (int, float)>();
    private float _swallowAtkInterval;

    public override FSMSystem System => raceter.SkillFsmSystem;

    private void Awake() { swallowFlipAnim.gameObject.Hide(); }

    public override void Init()
    {
        _swallowTrigger = swallowFlipAnim.GetComponent<TriggerEventCallback>();
        _swallowTrigger.OnTriggerEnterEvent += OnSwallowTriggerEvent;
        _swallowTrigger.OnTriggerStayEvent += OnSwallowTriggerEvent;
        raceter = transform.parent.GetComponent<EntityRaceter>();
        _playerMove = raceter.GetProperty<MoveCapability>();
        _swordResolve = raceter.GetProperty<SwordResolve>();
        _swallowFlipLength = SkillUtility.GetClipLength(swallowFlipAnim, Consts.PREFAB_SE_SkillRaceterSwallowFlip);
        transform.parent = raceter.SkillObjCollection.transform;
    }

    public override bool CanEnter() { return System.CurrentState.GetType() == typeof(NormalState) && IsCoolDown(); }

    public override void OnEnter()
    {
        if (_swordResolve.swordState)
        {
            _damage = raceter.CalculateDamage(60, DamageType.Physics);
            _swordResolve.PullSword();
            swallowFlipAnim.gameObject.Show();
            _playerMove.canMove = false;
            var face = (int) raceter.GetFace();
            var raceterTrans = raceter.transform;
            var swallowTrans = swallowFlipAnim.transform;
            var swallowScale = swallowTrans.localScale;
            swallowTrans.localScale = new Vector3(math.abs(swallowScale.x) * face, swallowScale.y, swallowScale.z);
            swallowTrans.position = raceterTrans.position;
            _animDuration = _swallowFlipLength;
            _swallowAtkInterval = _swallowFlipLength / 7f;
            swallowFlipAnim.Play(Consts.PREFAB_SE_SkillRaceterSwallowFlip, 0, 0);
        }
    }

    public override void OnAct()
    {
        _animDuration -= Time.deltaTime;
        if (_animDuration <= 0)
        {
            EnterStiffness(200);
        }
    }

    public override void OnLeave()
    {
        _playerMove.canMove = true;
        RefreshCoolDown();
        swallowFlipAnim.gameObject.Hide();
        _attacked.Clear();
    }

    private void OnSwallowTriggerEvent(Collider2D coll)
    {
        if (!coll.CompareTag(Consts.TAG_Entity))
        {
            return;
        }

        var e = coll.GetComponent<Entity>();
        if (e.EntityType != EntityType.Enemy)
        {
            return;
        }

        if (!(e is IAttackable attackable))
        {
            return;
        }

        if (_attacked.TryGetValue(coll, out var pair))
        {
            var count = pair.Item1;
            var time = pair.Item2;
            if (count >= 5)
            {
                return;
            }

            if (time >= _swallowAtkInterval)
            {
                attackable.UnderAttack(raceter.DealDamage(_damage, attackable));
                _attacked[coll] = (count + 1, 0);
            }
            else
            {
                _attacked[coll] = (count, time + Time.deltaTime);
            }
        }
        else
        {
            attackable.UnderAttack(raceter.DealDamage(_damage, attackable));
            _attacked.Add(coll, (0, 0));
        }
    }
}