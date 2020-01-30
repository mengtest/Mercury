using System;
using UnityEngine;

/// <summary>
/// 风刃
/// </summary>
public class SkillRaceterBladeWave : AbstractSkill
{
    private readonly IAttackable _playerAttack;
    private readonly EntityRaceter _raceter;

    public SkillRaceterBladeWave(ISkillable holder) : base(holder, 12)
    {
        _playerAttack = holder as IAttackable;
        _raceter = holder as EntityRaceter;
    }

    private static GameObject GetWave()
    {
        return GameManager.Instance.GetEffect(Consts.PREFAB_SE_SkillRaceterBladeWave).Hide();
    }

    public override bool CanEnter() { return CurrentSkill().GetType() == typeof(NormalState) && IsCoolDown(); }

    public override void OnAct() { EnterStiffness(0); }

    public override void OnEnter()
    {
        var loopCount = 1;
        var sr = _raceter.GetProperty<SwordResolve>();
        if (!sr.swordState)
        {
            loopCount += sr.resolve / 20;
        }

        var dmg = _raceter.CalculateDamage(150, DamageType.Physics);
        for (var i = 0; i < loopCount; i++)
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

                attackable.UnderAttack(_playerAttack.DealDamage(dmg, attackable));
                return true;
            };
            flight.OnDead += e =>
            {
                wave.Hide();
                GameManager.Instance.RecycleEffect(e.gameObject);
                flight.Reset();
            };
            flight.OnUpdate += e => e.transform.position += new Vector3(3f * Time.deltaTime, 0) * dir;
        }
    }

    public override void OnLeave() { RefreshCoolDown(); }
}