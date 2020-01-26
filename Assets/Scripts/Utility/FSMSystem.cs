using System;
using System.Collections.Generic;

public class FSMSystem
{
    private readonly Dictionary<Type, IFSMState> _states = new Dictionary<Type, IFSMState>();

    public IReadOnlyDictionary<Type, IFSMState> States => _states;

    public IFSMState CurrentState { get; set; }

    public FSMSystem(IFSMState defaultState)
    {
        AddState(defaultState);
        CurrentState = defaultState;
    }

    public void AddState(IFSMState state) { _states.Add(state.GetType(), state); }

    public bool RemoveState(Type type) { return _states.Remove(type); }

    public bool SwitchState(Type type) { return SwitchState(type, out _); }

    public bool SwitchState(Type type, out IFSMState state)
    {
        if (CurrentState == null)
        {
            state = default;
            return false;
        }

        state = _states[type];
        if (!state.CanEnter(CurrentState))
        {
            return false;
        }

        CurrentState.OnLeave();
        state.OnEnter();
        CurrentState = state;
        return true;
    }

    public void OnUpdate() { CurrentState.OnAct(); }
}