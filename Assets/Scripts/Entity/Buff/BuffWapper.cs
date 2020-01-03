﻿using System;
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

	public IBuff GetBuff(Type buffType, BuffVariant variant)
	{
		return SwitchWrapper(variant, (dict) => dict[buffType]);
	}

	public bool RemoveBuff(Type buffType, BuffVariant variant)
	{
		return SwitchWrapper(variant, (dict) => dict.Remove(buffType));
	}

	public bool TryGetBuff(Type buffType, BuffVariant variant, out IBuff buff)
	{
		switch (variant)
		{
			case BuffVariant.Dot:
				return _dotBuffs.TryGetValue(buffType, out buff);
			case BuffVariant.State:
				return _stateBuffs.TryGetValue(buffType, out buff);
			default:
				throw new ArgumentException();
		}
	}

	public bool HasBuff(Type buffType, BuffVariant variant)
	{
		return SwitchWrapper(variant, (dict) => dict.ContainsKey(buffType));
	}

	private TResult SwitchWrapper<TResult>(BuffVariant varian,Func<Dictionary<Type, IBuff>, TResult> func)
	{
		switch (varian)
		{
			case BuffVariant.Dot:
				return func(_dotBuffs);
			case BuffVariant.State:
				return func(_stateBuffs);
			default:
				throw new ArgumentException();
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
		rm.Clear();
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
