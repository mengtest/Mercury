public interface IFSMState
{
	/// <summary>
	/// 是否可以从当前激活的状态切换至本状态
	/// </summary>
	bool CanEnter(IFSMState current);
	/// <summary>
	/// 进入本状态时调用
	/// </summary>
	void OnEnter();
	/// <summary>
	/// 每帧调用
	/// </summary>
	void OnAct();
	/// <summary>
	/// 离开本状态时调用
	/// </summary>
	void OnLeave();
}
