using System;

public interface IBuffable
{
	void AddBuff(IBuff buff);

	/// <summary>
	/// 获取Buff，如果不存在会引发KeyNotFoundException异常
	/// </summary>
	/// <param name="buffType">Buff类</param>
	/// <param name="variant">Buff种类</param>
	IBuff GetBuff(Type buffType, BuffVariant variant);

	/// <summary>
	/// 尝试获取Buff
	/// </summary>
	/// <param name="buffType">Buff类</param>
	/// <param name="variant">Buff种类</param>
	/// <param name="buff">返回的Buff</param>
	/// <returns>该Buff是否存在</returns>
	bool TryGetBuff(Type buffType, BuffVariant variant, out IBuff buff);

	/// <summary>
	/// 查询Buff是否存在
	/// </summary>
	/// <param name="buffType">Buff类</param>
	/// <param name="variant">Buff种类</param>
	/// <returns>该Buff是否存在</returns>
	bool HasBuff(Type buffType, BuffVariant variant);

	/// <summary>
	/// 移除Buff
	/// </summary>
	/// <param name="buffType">Buff类</param>
	/// <param name="variant">Buff种类</param>
	/// <returns>是否移除成功</returns>
	bool RemoveBuff(Type buffType, BuffVariant variant);
}
