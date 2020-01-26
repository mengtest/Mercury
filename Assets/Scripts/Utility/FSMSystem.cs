using System;
using System.Collections.Generic;

/// <summary>
/// 有限状态机
/// </summary>
public class FSMSystem
{
    private readonly Dictionary<Type, IFSMState> _states = new Dictionary<Type, IFSMState>();

    /// <summary>
    /// 所有状态
    /// </summary>
    public IReadOnlyDictionary<Type, IFSMState> States => _states;

    /// <summary>
    /// 当前状态
    /// </summary>
    public IFSMState CurrentState { get; set; }

    /// <summary>
    /// 构造时需指定默认状态
    /// </summary>
    /// <param name="defaultState">默认状态</param>
    public FSMSystem(IFSMState defaultState)
    {
        AddState(defaultState);
        CurrentState = defaultState;
    }

    /// <summary>
    /// 添加状态
    /// </summary>
    /// <param name="state">状态实例</param>
    public void AddState(IFSMState state) { _states.Add(state.GetType(), state); }

    /// <summary>
    /// 移除状态
    /// </summary>
    /// <param name="type">状态类型</param>
    /// <returns>是否删除成功</returns>
    public bool RemoveState(Type type) { return _states.Remove(type); }

    /// <summary>
    /// 切换状态
    /// </summary>
    /// <param name="type">需要切换的状态类型</param>
    /// <returns>是否切换成功</returns>
    public bool SwitchState(Type type) { return SwitchState(type, out _); }

    /// <summary>
    /// 切换状态
    /// </summary>
    /// <param name="type">需要切换的状态类型</param>
    /// <param name="state">状态实例</param>
    /// <returns>是否切换成功</returns>
    public bool SwitchState(Type type, out IFSMState state)
    {
        if (CurrentState == null)
        {
            state = default;
            return false;
        }

        state = _states[type];
        if (!state.CanEnter())
        {
            return false;
        }

        CurrentState.OnLeave();
        state.OnEnter();
        CurrentState = state;
        return true;
    }

    /// <summary>
    /// 每帧调用当前状态
    /// </summary>
    public void OnUpdate() { CurrentState.OnAct(); }
}