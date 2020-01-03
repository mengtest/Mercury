using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 包装IBuffable接口
/// </summary>
public class BuffWapper
{
	private readonly IBuffable _buffHolder;
	private readonly Dictionary<Type, IBuff> _stateBuffs = new Dictionary<Type, IBuff>();
	private readonly Dictionary<Type, IBuff> _dotBuffs = new Dictionary<Type, IBuff>();

	public BuffWapper(IBuffable buffHolder)
	{
		_buffHolder = buffHolder;
	}

	public void AddBuff(IBuff buff)
	{
		if (_stateBuffs.TryGetValue(buff.GetType(), out var val))
		{
			val.Merge(buff);
		}
		else
		{
			if (buff.Variant == BuffVariant.Dot)
			{
				_dotBuffs.Add(buff.GetType(), buff);
			}
			else
			{
				_stateBuffs.Add(buff.GetType(), buff);
			}
			buff.OnFirstAdd(_buffHolder);
		}
	}

	private readonly List<(IBuff, Dictionary<Type, IBuff>)> rm = new List<(IBuff, Dictionary<Type, IBuff>)>();

	public void OnUpdate()
	{
		foreach (var buff in _dotBuffs.Values)
		{
			ReduceTime(buff, _dotBuffs, rm);
			buff.OnTrigger(_buffHolder);
		}
		foreach (var buff in _stateBuffs.Values)
		{
			ReduceTime(buff, _stateBuffs, rm);
		}
		foreach (var r in rm)
		{
			var (buff, dict) = r;
			dict.Remove(buff.GetType());
		}
	}

	private static void ReduceTime(IBuff buff, Dictionary<Type, IBuff> src, List<(IBuff, Dictionary<Type, IBuff>)> rmList)
	{
		if (buff.Duration != int.MaxValue)
		{
			buff.Duration -= Time.deltaTime;
		}
		if (buff.Duration <= 0)
		{
			rmList.Add(new ValueTuple<IBuff, Dictionary<Type, IBuff>>(buff, src));
		}
	}
}
