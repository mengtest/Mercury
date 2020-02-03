using System;
using UnityEngine;

/// <summary>
/// 狂风剑刃
/// </summary>
public class SkillRaceterBladeWave : AbstractSkill//TODO:技能作为GameObject存在
{
    private readonly IAttackable _playerAttack;
    private readonly EntityRaceter _raceter;
    private float _timeRecord;
    private int _launchCount;
    private Damage _damage;
    private readonly MoveCapability _move;

    public SkillRaceterBladeWave(ISkillable holder) : base(holder, 1)
    {
        _playerAttack = holder as IAttackable;
        _raceter = holder as EntityRaceter;
        _move = _raceter.GetProperty<MoveCapability>();
    }

    private static GameObject GetWave()
    {
        return GameManager.Instance.GetEffect(Consts.PREFAB_SE_SkillRaceterBladeWave).Hide();
    }

    public override bool CanEnter() { return CurrentSkill().GetType() == typeof(NormalState) && IsCoolDown(); }

    public override void OnAct()
    {
        if (_launchCount > 0)
        {
            _timeRecord += Time.deltaTime;
            if (_timeRecord >= 0.3f)
            {
                LaunchWave();
            }
        }
        else
        {
            EnterStiffness(0.5f);
        }

        _move.canMove = false;
    }

    public override void OnEnter()
    {
        var loopCount = 1;
        var sr = _raceter.GetProperty<SwordResolve>();
        if (!sr.swordState)
        {
            loopCount += sr.resolve / 20;
        }

        _launchCount = loopCount;
        _damage = _raceter.CalculateDamage(150, DamageType.Physics);
        LaunchWave();
    }

    public override void OnLeave() { RefreshCoolDown(); }

    private void LaunchWave()
    {
        var wave = GetWave();
        wave.Show();
        var flight = wave.GetComponent<EntityFlightProp>();
        flight.Reset();
        var transform = _raceter.transform;
        var dir = _raceter.GetFace() == Face.Left ? -1 : 1;
        wave.transform.position = transform.position;
        flight.Rotate(_raceter.GetFace());
        flight.IsDead += e =>
        {
            var t = e.Trigger;
            if (!t)
            {
                return false;
            }

            if (!t.CompareTag(Consts.TAG_Entity))
            {
                return false;
            }

            var entity = t.GetComponent<Entity>();
            if (entity.EntityType != EntityType.Enemy)
            {
                return false;
            }

            if (!(entity is IAttackable attackable))
            {
                throw new ArgumentException("未实现IAttackable却是Enemy");
            }

            attackable.UnderAttack(_playerAttack.DealDamage(_damage, attackable));
            return true;
        };
        flight.OnDead += e =>
        {
            wave.Hide();
            GameManager.Instance.RecycleEffect(e.gameObject);
            flight.Reset();
        };
        flight.OnUpdateAction += e => e.transform.position += new Vector3(3f * Time.deltaTime, 0) * dir;
        _launchCount -= 1;
        _timeRecord = 0f;
    }
}