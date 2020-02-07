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
    private float _lastDamageUpgrade;
    private float _lastAtkTime;
    private float _lastCritProAdd;
    private bool _isRetractSwordState;
    private float _retractTime;

    public bool IsResolveFull => resolve >= 100;

    public SwordResolve(EntityRaceter raceter)
    {
        _raceter = raceter;
        _raceter.OnAttackTarget += (dmg, target) => //攻击敌人时
        {
            if (swordState) //拔刀状态
            {
                OverlayResolve(2); //增加2剑意
                RefuseCritPro(); //刷新暴击率
                _lastAtkTime = Time.time; //重置攻击敌人计时器
            }
        };
    }

    public void OnUpdate()
    {
        if (_isRetractSwordState)
        {
            _retractTime += Time.deltaTime;
            if (_retractTime >= 1) //切换为收刀
            {
                Retract();
            }
            else
            {
                return;
            }
        }

        _timeOverlay += Time.deltaTime; //拔刀时表示每隔0.5s剑意减少，收刀时表示每隔1s剑意增加
        if (swordState)
        {
            var now = Time.time;
            if (now - _lastAtkTime >= 1f) //距离上次攻击超过1s
            {
                if (_timeOverlay >= 0.5f) //距上次剑意减少超过0.5s
                {
                    OverlayResolve(-5);
                    RefuseCritPro();
                    _timeOverlay = 0f;
                }
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                _isRetractSwordState = true;
            }
        }
        else
        {
            if (_timeOverlay >= 1f) //距上次剑意增加超过1s
            {
                OverlayResolve(20);
                RefuseDamageUpgrade();
                _timeOverlay = 0f;
            }
        }
    }

    public void OverlayResolve(int point)
    {
        if (IsResolveFull)
        {
            return;
        }

        var tryAdd = resolve + point;
        resolve = math.max(0, math.min(100, tryAdd));
    }

    /// <summary>
    /// 拔刀
    /// </summary>
    public void PullSword()
    {
        if (!swordState) //收刀状态
        {
            RemoveDamageUpgrade(); //移除伤害增益
            resolve /= 2; //剑意减半
            swordState = true; //切换为拔刀状态
            _timeOverlay = 0f; //重置计时器
            _lastDamageUpgrade = 0f; //重置增伤累加
        }
    }
    
    /// <summary>
    /// 收刀
    /// </summary>
    public void Retract()
    {
        if (swordState)
        {
            RemoveCritPro();
            resolve = 0;
            swordState = false;
            _timeOverlay = 0f;
            _lastCritProAdd = 0f;
            _isRetractSwordState = false;
            _retractTime = 0f;
        }
    }

    private void RefuseDamageUpgrade()
    {
        var coe = resolve * 0.005f + 1;
        if (math.abs(coe - _lastDamageUpgrade) < 0.001f)
        {
            return;
        }

        RemoveDamageUpgrade();
        _raceter.DamageCalculator.AddDamageChain(new DamageChain(DamageIncome.Upgrade,
            _raceter,
            coe,
            DamageType.True));
        _lastDamageUpgrade = coe;
    }

    private void RemoveDamageUpgrade()
    {
        _raceter.DamageCalculator.RemoveDamageChain(new DamageChain(DamageIncome.Upgrade,
            _raceter,
            _lastDamageUpgrade,
            DamageType.True));
    }

    private void RemoveCritPro()
    {
        _raceter.DamageCalculator.RemoveCritProbabilityChain(
            new DamageCritProbabilityChain(_lastCritProAdd, CritProbabilityIncomeType.Percentage, _raceter));
    }

    private void RefuseCritPro()
    {
        var pro = resolve * 0.001f;
        if (math.abs(pro - _lastCritProAdd) < 0.001f)
        {
            return;
        }

        RemoveCritPro();
        _raceter.DamageCalculator.AddCritProbabilityChain(
            new DamageCritProbabilityChain(pro, CritProbabilityIncomeType.Percentage, _raceter));
        _lastCritProAdd = pro;
    }
}