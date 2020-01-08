using System;
using System.Collections.Generic;

public class FSMSystem<T> where T : IFSMState
{
	private readonly Dictionary<Type, T> _states = new Dictionary<Type, T>();

	public T CurrentState { get; set; }

	public void AddState(T state)
	{
		_states.Add(state.GetType(), state);
	}

	public bool RemoveState(Type type)
	{
		return _states.Remove(type);
	}

	public bool SwitchState(Type type)
	{
		if (CurrentState == null)
		{
			return false;
		}
		var state = _states[type];
		if (!state.CanEnter(CurrentState))
		{
			return false;
		}
		CurrentState.OnLeave();
		state.OnEnter();
		CurrentState = state;
		return true;
	}

	public void OnUpdate()
	{
		CurrentState.OnAct();
	}
}
