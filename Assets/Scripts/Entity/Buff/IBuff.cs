public interface IBuff
{
	/// <summary>
	/// 剩余持续时间
	/// </summary>
	float Duration { get; set; }
	/// <summary>
	/// 强度
	/// </summary>
	int Intensity { get; }
	/// <summary>
	/// 种类
	/// </summary>
	BuffVariant Variant { get; }

	/// <summary>
	/// 合并Buff时触发
	/// </summary>
	/// <param name="other">要与之合并的Buff</param>
	void Merge(IBuff other);

	/// <summary>
	/// 第一次添加Buff时触发
	/// </summary>
	void OnFirstAdd(IBuffable buffable);

	/// <summary>
	/// 移除Buff时触发
	/// </summary>
	void OnRemove(IBuffable buffable);

	/// <summary>
	/// 当Buff触发时调用
	/// </summary>
	void OnTrigger(IBuffable buffable);

	/// <summary>
	/// Buff是否可以触发
	/// </summary>
	bool IsReady();
}
