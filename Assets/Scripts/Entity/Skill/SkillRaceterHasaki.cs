using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 气旋斩
/// </summary>
public class SkillRaceterHasaki : AbstractSkill
{
    private readonly Stack<GameObject> _hasakis = new Stack<GameObject>();
    private readonly float _animTime;
    private float _duration;
    private readonly IAttackable _playerAttack;
    private readonly EntityPlayer _player;

    public SkillRaceterHasaki(ISkillable holder) : base(holder, 1)
    {
        _hasakis.Push(GetHasaki(true));
        var hasaki = _hasakis.Peek();
        _animTime = GetClipLength(hasaki.GetComponent<Animator>(), Consts.PREFAB_SE_SkillRaceterHasaki);
        _duration = _animTime;
        _player = holder as EntityPlayer;
        _playerAttack = holder as IAttackable;
        hasaki.Hide();
    }

    private GameObject GetHasaki(bool show = false)
    {
        var go = GameManager.Instance.GetEffect(Consts.PREFAB_SE_SkillRaceterHasaki);
        go.SetActive(show);
        return go;
    }

    public override bool CanEnter()
    {
        return CurrentSkill().GetType() == typeof(NormalState) && IsCoolDown();
    }

    public override void OnEnter()
    {
        var hasaki = _hasakis.Count == 0 ? GetHasaki() : _hasakis.Pop();
        hasaki.Show();
        var flight = hasaki.GetComponent<EntityFlightProp>();
        flight.Reset();
        var transform = _player.transform;
        var dir = _player.GetFace() == Face.Left ? -1 : 1;
        hasaki.transform.position = transform.position;
        flight.Rotate(_player.GetFace());
    }

    public override void OnLeave() { throw new System.NotImplementedException(); }

    public override void OnAct() { throw new System.NotImplementedException(); }
}