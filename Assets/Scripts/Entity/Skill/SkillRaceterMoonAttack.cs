using System;
using System.Collections.Generic;
using UnityEngine;

// [AutoRegister("raceter_moon_atk",
//     "skill.raceter_moon_atk",
//     "skill.raceter_moon_atk_2")]
public class SkillRaceterMoonAttack : AbstractSkill
{
    private readonly EntityRaceter _raceter;
    private readonly SwordResolve _swordResolve;
    private readonly MoveCapability _move;
    [Inject] private GameObject _atkRng = null;
    [Inject] private Asset _atk2Asset = null;
    private float _animEndTime;
    private Stack<GameObject> _atk2Pool = new Stack<GameObject>(4);
    private List<GameObject> _activeAtk2 = new List<GameObject>(4);

    public override AssetLocation RegisterName { get; } = Consts.SkillRaceterMoonAttack;

    public SkillRaceterMoonAttack(ISkillable user) : base(user)
    {
        if (!(user is EntityRaceter raceter))
        {
            throw new InvalidOperationException();
        }

        _raceter = raceter;
        _swordResolve = raceter.GetProperty<SwordResolve>();
        _move = raceter.GetProperty<MoveCapability>();
    }

    public override void Init()
    {
        _atkRng.transform.parent = _raceter.SkillCollection.transform;
        var callback = _atkRng.AddComponent<TriggerEventCallback>();
        callback.OnTriggerEnterEvent += OnTriggerEvent;
        callback.OnTriggerStayEvent += OnTriggerEvent;
        callback.gameObject.Hide();
    }

    public override bool CanEnter() { return true; }

    public override void OnEnter()
    {
        _atkRng.Show();
        _atkRng.transform.position = _raceter.transform.position + new Vector3(0, 2, 0);
        _animEndTime = Time.time + 2;
    }

    public override void OnUpdate()
    {
        _atkRng.transform.position = _raceter.transform.position + new Vector3(0, 2, 0);
        if (Time.time >= _animEndTime)
        {
            _raceter.UseSkill(Consts.SkillStiffness, out var skill);
            ((StiffnessState) skill).ExpireTime = 0;
        }
    }

    public override void OnLeave()
    {
        foreach (var atk2 in _activeAtk2)
        {
            atk2.Hide();
            _atk2Pool.Push(atk2);
        }

        _activeAtk2.Clear();
        _atkRng.Hide();
    }

    private GameObject GetAtk2() { return _atk2Pool.Count == 0 ? _atk2Asset.Instantiate() : _atk2Pool.Pop(); }

    private void OnTriggerEvent(Collider2D coll)
    {
        Debug.Log(coll.name);
        if (!coll.CompareTag(Consts.Entity))
        {
            return;
        }
        
        var go = GetAtk2();
        go.Show();
        _activeAtk2.Add(go);
    }
}