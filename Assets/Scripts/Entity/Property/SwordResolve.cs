using System;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// 剑意
/// </summary>
[Serializable]
public class SwordResolve : IEntityProperty
{
    /// <summary>
    /// 刀状态，true为拔刀，false为收刀
    /// </summary>
    public bool swordState;

    /// <summary>
    /// 剑意
    /// </summary>
    public int resolve;

    private readonly EntityRaceter _raceter;
    private float _timeOverlay;
    private float _lastAtkAdd;

    public bool IsResolveFull => resolve >= 100;

    public SwordResolve(EntityRaceter raceter)
    {
        _raceter = raceter;
        _raceter.OnAttackTarget += (dmg, target) =>
        {
            if (swordState)
            {
                //TODO:拔刀状态
            }
            else
            {
                RemoveDamageUpgrade();
                resolve /= 2;
                swordState = true;
            }
        };
    }

    public void OnUpdate()
    {
        _timeOverlay += Time.deltaTime;
        if (swordState)
        {
            //TODO:拔刀状态
        }
        else
        {
            if (_timeOverlay >= 1f)
            {
                AddResolve(20);
                AddDamageUpgrade();
                _timeOverlay = 0f;
            }
        }
    }

    public void AddResolve(int point)
    {
        if (IsResolveFull)
        {
            return;
        }

        var tryAdd = resolve + point;
        resolve = math.min(100, tryAdd);
    }

    private void AddDamageUpgrade()
    {
        var coe = resolve * 0.005f + 1;
        if (math.abs(coe - _lastAtkAdd) < 0.001f)
        {
            return;
        }

        RemoveDamageUpgrade();
        _raceter.DamageCalculator.AddDamageChain(new DamageChain(DamageIncome.Upgrade,
            _raceter,
            coe,
            DamageType.True));
        _lastAtkAdd = coe;
    }

    private void RemoveDamageUpgrade()
    {
        _raceter.DamageCalculator.RemoveDamageChain(new DamageChain(DamageIncome.Upgrade,
            _raceter,
            _lastAtkAdd,
            DamageType.True));
    }
}