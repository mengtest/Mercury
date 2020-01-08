using System;
using UnityEngine;

/// <summary>
/// 基础状态
/// </summary>
[Serializable]
public class BasicState : IEntityProperty
{
	public bool isOnStep;
	public Collision2D standedStep;
}
