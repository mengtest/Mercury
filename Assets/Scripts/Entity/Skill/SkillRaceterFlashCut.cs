using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// 烈
/// </summary>
public class SkillRaceterFlashCut : SkillObject
{
    public EntityRaceter raceter;
    // public AssetReference flashCut;
    private GameObject _flashCutPrefab;
    private MoveCapability _move;
    private Stack<Animator> _flashPool;
    private Queue<Animator> _activeFlash;
    private Queue<IBuffable> _rmE;
    private SwordResolve _swordResolve;
    private float _animLength;
    private float _expireTime;
    private int _srAdd;

    public override FSMSystem System => raceter.SkillFsmSystem;

    private void OnDestroy()
    {
        foreach (var wave in _flashPool)
        {
            Destroy(wave.gameObject);
        }

        _flashPool = null;
    }

    public override async void Init()
    {
        raceter = transform.parent.GetComponent<EntityRaceter>();
        _move = raceter.GetProperty<MoveCapability>();
        _swordResolve = raceter.GetProperty<SwordResolve>();
        _flashPool = new Stack<Animator>(1);
        _activeFlash = new Queue<Animator>();
        // _flashCutPrefab = await flashCut.LoadAssetAsync<GameObject>().Task;
        var anim = GetFlashCut();
        anim.gameObject.Hide();
        _animLength = SkillUtility.GetClipLength(anim, Consts.PREFAB_SE_SkillRaceterFlashCut);
        _flashPool.Push(anim);
        _rmE = new Queue<IBuffable>();
        transform.parent = raceter.SkillObjCollection.transform;
    }

    private Animator GetFlashCut()
    {
        var r = _flashPool.Count != 0
            ? _flashPool.Pop()
            : Instantiate(_flashCutPrefab, transform, true).GetComponent<Animator>();
        return r;
    }

    public override bool CanEnter() { return System.CurrentState.GetType() == typeof(NormalState) && IsCoolDown(); }

    public override void OnEnter()
    {
        _expireTime = Time.time + _animLength;
        var dmg = raceter.CalculateDamage(400, DamageType.True);
        var srCount = 0;
        foreach (var entity in raceter.HasWindMarkBuff)
        {
            var buf = entity as IBuffable;
            var atk = entity as IAttackable;
            var wm = buf.GetStateBuff(Consts.BUFF_WindMark);
            srCount = math.max(srCount, wm.intensity);
            atk.UnderAttack(raceter.DealDamage(dmg, atk));
            var anim = GetFlashCut();
            anim.gameObject.Show();
            anim.transform.position = entity.transform.position;
            anim.Play(Consts.PREFAB_SE_SkillRaceterFlashCut, 0, 0);
            _activeFlash.Enqueue(anim);
            _rmE.Enqueue(buf);
        }

        while (_rmE.Count != 0)
        {
            _rmE.Dequeue().RemoveStateBuff(Consts.BUFF_WindMark);
        }

        _rmE.Clear();
        raceter.HasWindMarkBuff.Clear();
        _srAdd = srCount * 5;
        _move.canMove = false;
    }

    public override void OnUpdate()
    {
        if (Time.time >= _expireTime)
        {
            EnterStiffness(0);
        }
    }

    public override void OnLeave()
    {
        _swordResolve.swordState = false;
        _swordResolve.OverlayResolve(_srAdd);
        _swordResolve.Retract();
        while (_activeFlash.Count != 0)
        {
            var t = _activeFlash.Dequeue();
            t.gameObject.Hide();
            _flashPool.Push(t);
        }
    }
}