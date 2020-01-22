using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 风刃
/// </summary>
public class SkillRaceterBladeWave : AbstractSkill
{
    private readonly Stack<GameObject> _waves = new Stack<GameObject>(1);
    private readonly IAttackable _playerAttack;
    private readonly EntityPlayer _player;

    public SkillRaceterBladeWave(ISkillable holder) : base(holder, 12)
    {
        _waves.Push(GetWave());
        _playerAttack = holder as IAttackable;
        _player = holder as EntityPlayer;
    }

    ~SkillRaceterBladeWave()
    {
        while (_waves.Count != 0)
        {
            GameManager.Instance.RecycleEffect(_waves.Pop());
        }
    }

    private static GameObject GetWave()
    {
        return GameManager.Instance.GetEffect(Consts.PREFAB_SE_SkillRaceterBladeWave).Hide();
    }

    public override bool CanEnter(IFSMState current)
    {
        return current.GetType() == typeof(NormalState) && IsCoolDown();
    }

    public override void OnAct() { EnterStiffness(0); }

    public override void OnEnter()
    {
        var wave = _waves.Count == 0 ? GetWave() : _waves.Pop();
        wave.Show();
        var flight = wave.GetComponent<EntityFlightProp>();
        flight.Reset();
        var transform = _player.transform;
        var dir = _player.GetFace() == Face.Left ? -1 : 1;
        wave.transform.position = transform.position;
        flight.Rotate(_player.GetFace());
        flight.isDead += e =>
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

            attackable.UnderAttack(_playerAttack.DealDamage(1, DamageType.Physics));
            //TODO:剑意
            return true;
        };
        flight.onDead += e =>
        {
            wave.Hide();
            _waves.Push(e.gameObject);
        };
        flight.onUpdate += e => e.transform.position += new Vector3(3f * Time.deltaTime, 0) * dir;
    }

    public override void OnLeave() { }
}