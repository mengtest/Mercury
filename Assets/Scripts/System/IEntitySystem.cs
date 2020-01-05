public interface IEntitySystem
{
	/// <summary>
	/// 是否需要物理运算
	/// </summary>
	bool IsPhysic { get; }
	/// <summary>
	/// 每帧调用
	/// </summary>
	void OnUpdate(Entity entity);
}
