using System.Collections.Generic;

/// <summary>
/// 有限状态机
/// </summary>
public class FSMSystem
{
    private readonly Dictionary<string, IFSMState> _states = new Dictionary<string, IFSMState>();

    /// <summary>
    /// 所有状态
    /// </summary>
    public IReadOnlyDictionary<string, IFSMState> States => _states;

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

    public FSMSystem() { CurrentState = null; }

    /// <summary>
    /// 添加状态
    /// </summary>
    /// <param name="state">状态实例</param>
    public void AddState(IFSMState state) { _states.Add(state.RegisterName.ToString(), state); }

    /// <summary>
    /// 移除状态
    /// </summary>
    /// <param name="stateName">状态名字</param>
    /// <returns>是否删除成功</returns>
    public bool RemoveState(string stateName) { return _states.Remove(stateName); }

    /// <summary>
    /// 切换状态
    /// </summary>
    /// <param name="stateName">需要切换的状态类型</param>
    /// <returns>是否切换成功</returns>
    public bool SwitchState(string stateName)
    {
        var state = _states[stateName];
        if (!state.CanEnter())
        {
            return false;
        }

        CurrentState?.OnLeave();
        state.OnEnter();
        CurrentState = state;
        return true;
    }

    /// <summary>
    /// 切换状态
    /// </summary>
    /// <param name="stateName">需要切换的状态类型</param>
    /// <param name="state">状态实例</param>
    /// <returns>是否切换成功</returns>
    public bool SwitchState(string stateName, out IFSMState state)
    {
        state = _states[stateName];
        if (!state.CanEnter())
        {
            return false;
        }

        CurrentState?.OnLeave();
        state.OnEnter();
        CurrentState = state;
        return true;
    }

    /// <summary>
    /// 每帧调用当前状态
    /// </summary>
    public void OnUpdate() { CurrentState.OnUpdate(); }
}