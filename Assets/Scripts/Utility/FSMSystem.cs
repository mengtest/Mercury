using System;
using System.Collections.Generic;

public class FSMSystem<T> where T : IFSMState
{
    private readonly Dictionary<Type, T> _states = new Dictionary<Type, T>();

    public IReadOnlyDictionary<Type, T> States => _states;

    public T CurrentState { get; set; }

    public void AddState(T state) { _states.Add(state.GetType(), state); }

    public bool RemoveState(Type type) { return _states.Remove(type); }

    public bool SwitchState(Type type) { return SwitchState(type, out _); }

    public bool SwitchState(Type type, out T state)
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