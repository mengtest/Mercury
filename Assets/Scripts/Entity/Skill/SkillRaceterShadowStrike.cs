using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// 斩影
/// </summary>
public class SkillRaceterShadowStrike : SkillObject, IFSMState
{
    public Animator skillAnimator;
    public EntityPlayer player;
    private MoveCapability _playerMove;
    private float _animLength;
    private float _duration;
    private float _move;
    private float _lastMove;
    private readonly HashSet<IAttackable> _attacked = new HashSet<IAttackable>();

    public FSMSystem System => player.SkillFsmSystem;

    public void Init()
    {
        player = transform.parent.GetComponent<EntityPlayer>();
        _playerMove = player.GetProperty<MoveCapability>();
        _animLength = SkillUtility.GetClipLength(skillAnimator, Consts.PREFAB_SE_SkillRaceterShadowStrike);
        _duration = _animLength;
        transform.parent = null;
    }

    public bool CanEnter() { return System.CurrentState.GetType() == typeof(NormalState) && IsCoolDown(); }

    public void OnAct()
    {
        _duration -= Time.deltaTime;
        if (_duration <= 0)
        {
            System.SwitchState<StiffnessState>(out var state);
            state.Duration = 200;
            return;
        }

        var v = new Vector2(_lastMove - _move, 0);
        player.Move(v);
        _lastMove = _move;
        if (Contact)
        {
            var e = Contact.GetComponent<Entity>();
            if (e.EntityType != EntityType.Enemy)
            {
                return;
            }

            if (e is IAttackable attackable)
            {
                if (!_attacked.Contains(attackable))
                {
                    attackable.UnderAttack(player.DealDamage(player.CalculateDamage(95, DamageType.Physics),
                        attackable));
                    _attacked.Add(attackable);
                }
            }
        }
    }

    public void OnEnter()
    {
        gameObject.Show();
        player.Velocity = Vector2.zero;
        _playerMove.canMove = false;
        if (player.GetFace() == Face.Left)
        {
            DifferentDir(180, -1);
        }
        else if (player.GetFace() == Face.Right)
        {
            DifferentDir(0, 1);
        }

        skillAnimator.Play(Consts.PREFAB_SE_SkillRaceterShadowStrike, 0, 0);
    }

    private void DifferentDir(float y, int coe)
    {
        var t = transform;
        t.eulerAngles = new Vector2(0, y);
        t.position = player.transform.position + new Vector3(3 * coe, -0.2f);
        _move = 0f;
        _lastMove = 0f;
        DOTween.To(() => _move, v => _move = v, 4f * -coe, _animLength).SetEase(Ease.OutExpo);
    }

    public void OnLeave()
    {
        _playerMove.canMove = true;
        _playerMove.velocity = Vector2.zero;
        _duration = _animLength;
        _attacked.Clear();
        RefreshCoolDown();
        gameObject.Hide();
    }
}