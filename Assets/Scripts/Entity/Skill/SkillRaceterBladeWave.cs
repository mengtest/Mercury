using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

/// <summary>
/// 狂风剑刃
/// </summary>
public class SkillRaceterBladeWave : SkillObject
{
    public EntityRaceter raceter;
    public AssetReference bladeWave;
    private GameObject _wavePrefab;
    private Stack<EntityFlightProp> _wavePool;
    private MoveCapability _move;
    private SwordResolve _swordResolve;
    private Damage _damage;
    private float _timeRecord;
    private int _launchCount;

    public override FSMSystem System => raceter.SkillFsmSystem;

    private void OnDestroy()
    {
        foreach (var wave in _wavePool)
        {
            Destroy(wave.gameObject);
        }

        _wavePool = null;
    }

    public override async void Init()
    {
        raceter = transform.parent.GetComponent<EntityRaceter>();
        _move = raceter.GetProperty<MoveCapability>();
        _swordResolve = raceter.GetProperty<SwordResolve>();
        _wavePool = new Stack<EntityFlightProp>(5);
        _wavePrefab = await bladeWave.LoadAssetAsync<GameObject>().Task;
        transform.parent = raceter.SkillObjCollection.transform;
    }

    private EntityFlightProp GetBladeWave() //不知道为什么，异步加载的Task，手动Wait会炸掉...
    {
        return _wavePool.Count != 0
            ? _wavePool.Pop()
            : Instantiate(_wavePrefab, transform, true).GetComponent<EntityFlightProp>();
    }

    public override bool CanEnter() { return System.CurrentState.GetType() == typeof(NormalState) && IsCoolDown(); }

    public override void OnUpdate()
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
            EnterStiffness(0);
        }

        _move.canMove = false;
    }

    public override void OnEnter()
    {
        var loopCount = 1;
        if (!_swordResolve.swordState)
        {
            loopCount += _swordResolve.resolve / 20;
        }

        _launchCount = loopCount;
        _damage = raceter.CalculateDamage(150, DamageType.Physics);
        _swordResolve.PullSword();
        LaunchWave();
    }

    public override void OnLeave() { RefreshCoolDown(); }

    private void LaunchWave()
    {
        var flight = GetBladeWave();
        flight.gameObject.Show();
        flight.Reset();
        var playerTrans = raceter.transform;
        var dir = raceter.GetFace() == Face.Left ? -1 : 1;
        flight.transform.position = playerTrans.position;
        flight.Rotate(raceter.GetFace());
        flight.IsDead += e =>
        {
            var t = e.Trigger;
            if (!t)
            {
                return false;
            }

            if (!t.CompareTag(Consts.Entity))
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

            attackable.UnderAttack(raceter.DealDamage(_damage, attackable));
            return true;
        };
        flight.OnDead += e =>
        {
            e.gameObject.Hide();
            _wavePool.Push(e);
        };
        flight.OnUpdateAction += e => e.transform.position += new Vector3(3f * Time.deltaTime, 0) * dir;
        _launchCount -= 1;
        _timeRecord = 0f;
    }
}