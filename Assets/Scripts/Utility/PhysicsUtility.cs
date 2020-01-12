using Unity.Mathematics;
using UnityEngine;

public static class PhysicsUtility
{
	public static RaycastHit? Raycast(float3 pos, float3 dir, float length)
	{
		return Physics.Raycast(pos, math.normalize(dir), out var hit, length) ? new RaycastHit?(hit) : null;
	}

	/// <summary>
	/// 2D射线碰撞检测
	/// </summary>
	/// <param name="pos">开始位置</param>
	/// <param name="dir">射线方向</param>
	/// <param name="length">射线长度</param>
	/// <returns>如果碰撞则返回碰撞结果，否则返回null</returns>
	public static RaycastHit2D? Raycast2D(float2 pos, float2 dir, float length)
	{
		var hit = Physics2D.Raycast(pos, math.normalize(dir), length);
		return hit.collider == null ? null : new RaycastHit2D?(hit);
	}

	/// <summary>
	/// 检测墙
	/// </summary>
	/// <param name="pos">开始位置</param>
	/// <param name="dir">射线方向</param>
	/// <param name="length">射线长度</param>
	/// <returns>-1：撞到东西但不是墙，0：没撞到任何东西，1：实心墙，2：可穿越墙</returns>
	public static int HitWall2D(float2 pos, float2 dir, float length)
	{
		var hit = Raycast2D(pos, dir, length);
		if (!hit.HasValue)
		{
			return 0;
		}
		var v = hit.Value;
		if (v.transform.CompareTag("Step"))
		{
			return 2;
		}
		if (v.transform.CompareTag("StaticWall"))
		{
			return 1;
		}
		return -1;
	}
}
